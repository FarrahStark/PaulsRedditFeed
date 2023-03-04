using Microsoft.AspNetCore.SignalR;

namespace PaulsRedditFeed
{
    public class RedditStatsProcessor : BackgroundService
    {
        private readonly IHubContext<RedditStatsHub> statsHub;
        private readonly ILogger<RedditStatsProcessor> logger;
        private readonly ConnectionMultiplexer redis;
        private readonly AppSettings settings;
        private ISubscriber messageQueue;

        public RedditStatsProcessor(
            IHubContext<RedditStatsHub> statsHub,
            ILogger<RedditStatsProcessor> logger,
            ConnectionMultiplexer redis,
            AppSettings settings)
        {
            this.statsHub = statsHub;
            this.logger = logger;
            this.redis = redis;
            this.settings = settings;
            this.messageQueue = redis.GetSubscriber();
        }

        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (stoppingToken.IsCancellationRequested) { return; }
            logger.LogInformation($"{nameof(RedditStatsProcessor)} Started");
            logger.LogInformation("Subscribing to the redis stats queue");
            messageQueue.Subscribe(settings.Redis.QueueChannelName).OnMessage(async payload =>
            {
                logger.LogInformation("Message recieved from queue. Processing...");
                SubredditDataMessage? message = null;
                try
                {
                    message = JsonSerializer.Deserialize<SubredditDataMessage>(payload.Message);
                }
                catch (Exception ex)
                {
                    logger.LogError("Unable to deserialize queue message", ex);
                }

                if (message == null)
                {
                    logger.LogError($"Unable to deserialize {nameof(SubredditDataMessage)}: {payload.Message}");
                }
                else
                {
                    var topPost = message.HotPosts.data.children.FirstOrDefault();

                    var viewModel = new SubredditStatsViewModel
                    {
                        Title = message.Subreddit.data.display_name,
                        ActiveUserCount = message.Subreddit.data.active_user_count,
                        TopPostTitle = topPost?.data?.title ?? "No title found!",
                        TopPostUpvotes = topPost?.data?.ups ?? 0,
                        TopPostDownvotes = topPost?.data?.downs ?? 0,
                    };
                    await NotifyClientsAsync(viewModel);
                    logger.LogInformation($"Notified clients of updates in r/{viewModel.Title}");
                }
            });
        }

        public async Task NotifyClientsAsync<T>(T payload)
        {
            await statsHub.Clients.All.SendAsync("ReceiveMessage", JsonSerializer.Serialize(payload));
        }

        public override void Dispose()
        {
            logger.LogInformation($"{nameof(RedditStatsProcessor)} is shutting down. Unsubscribing from redis queue");
            messageQueue.Unsubscribe(settings.Redis.QueueChannelName);
            base.Dispose();
        }
    }
}
