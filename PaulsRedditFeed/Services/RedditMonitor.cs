using Microsoft.AspNetCore.SignalR;
using Reddit;
using StackExchange.Redis;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PaulsRedditFeed
{
    public class RedditMonitor : BackgroundService
    {
        private static readonly Random random = new Random();
        private readonly ILogger<RedditMonitor> logger;
        private readonly RedditClient reddit;
        private readonly IHubContext<RedditStatsHub> statsHub;
        private readonly ConnectionMultiplexer redis;
        private readonly HttpClient httpClient;
        private readonly AppSettings settings;
        private static readonly string MonitoredSubreddits = "monitored_subreddits";
        private static readonly string users = "users";

        public RedditMonitor(
            ILogger<RedditMonitor> logger,
            RedditClient reddit,
            ConnectionMultiplexer redis,
            HttpClient httpClient,
            AppSettings settings)
        {
            this.logger = logger;
            this.reddit = reddit;
            this.redis = redis;
            this.httpClient = httpClient;
            this.settings = settings;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            SeedCache();
            logger.LogInformation($"{nameof(RedditMonitor)} Started");
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await MonitorAllSubreddits(stoppingToken);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Subreddits scan failed");
                }
                finally
                {
                    await Task.Delay(settings.PollingIntervalMilliseconds);
                }
            }
            logger.LogInformation($"{nameof(RedditMonitor)} Stopped");
        }

        /// <summary>
        /// Fetches updated data for each actively monitored subreddit and queues it for processing.
        /// Uses the TPL for efficient thread usage instead of trying to manage the threads
        /// manually.
        /// </summary>
        /// <param name="stoppingToken">Allows the async operations to be cancelled</param>
        /// <returns>a task tracking the async operation</returns>
        private async Task MonitorAllSubreddits(CancellationToken stoppingToken)
        {
            logger.LogInformation("Scanning all subreddits");
            // Get updated info about monitored subreddits
            var subredditSubscriptions = await redis.GetDatabase()
                .HashGetAllAsync(settings.Redis.SubredditSubscriptionKey);

            // Publish raw subreddit data to queue
            var subredditMonitoringTasks = subredditSubscriptions
                .Where(subreddit => int.Parse(subreddit.Value) > 0)
                .Select(subreddit => Task.Run(() => MonitorSubreddit(subreddit.Key, stoppingToken)));

            await Task.WhenAll(subredditMonitoringTasks);
            logger.LogInformation("All subreddits scan completed");
        }

        private async Task MonitorSubreddit(string subredditName, CancellationToken stoppingToken)
        {
            try
            {
                logger.LogInformation($"Scanning subreddit {subredditName}");

                // Collect subreddit data from Reddit API and Queue up
                var subreddit = await Task.Run(() => reddit.Subreddit(subredditName).About());
                var hottestPost = subreddit.Posts.GetHot(limit: 1).OrderByDescending(post => post.Score).First();
                var dataToCache = new SubredditRawData(DateTime.UtcNow, subreddit, hottestPost);
                string dataJson = string.Empty;

                dataJson = JsonSerializer.Serialize(dataToCache, new JsonSerializerOptions
                {
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                    IgnoreReadOnlyProperties = true
                });

                var messageQueue = redis.GetSubscriber();

                await messageQueue.PublishAsync(settings.Redis.QueueChannelName, dataJson);


                logger.LogInformation($"Scan complete {subredditName}");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Scanning of subreddit {subredditName} failed.");
                throw;
            }
        }

        /// <summary>
        /// Simulates user account creation and subreddit subscriptions by seeding users into the cache
        /// This will overwrite existing values in the cache
        /// </summary>
        /// <param name="redis">allows connections to redis</param>
        private void SeedCache()
        {
            var users = new User[]
            {
                new User { Id = 1, SubscribedSubreddits = new List<String> { "Baking", "DIY" } },
                new User { Id = 2, SubscribedSubreddits = new List<String> { "lego", "programming", "AskReddit" } },
                new User { Id = 3, SubscribedSubreddits = new List<String> { "AskReddit", "Space", "Aww" } },
                new User { Id = 4, SubscribedSubreddits = new List<String> { "Music", "DIY", "Space", "AskReddit" } },
            };

            var subreddits = users.SelectMany(u => u.SubscribedSubreddits);
            var subscriberCounts = new Dictionary<string, int>();

            foreach (var subreddit in subreddits)
            {
                if (!subscriberCounts.ContainsKey(subreddit))
                {
                    subscriberCounts[subreddit] = 0;
                }

                subscriberCounts[subreddit]++;
            }

            var subscriptions = subscriberCounts
                .Select(kvp =>
                {
                    var subredditKey = kvp.Key;
                    var subscriberCount = kvp.Value;
                    var subscription = new SubredditSubscription
                    {
                        Subreddit = subredditKey,
                        SubscriberCount = subscriberCount,
                    };

                    var subscriptionJson = JsonSerializer.Serialize(subscription);
                    return new HashEntry(subredditKey, subscriberCount);
                }).ToArray();

            var userEntries = users.Select(user =>
            {
                var userJson = JsonSerializer.Serialize(user);
                return new HashEntry(user.Id, userJson);
            }).ToArray();

            var db = redis.GetDatabase();
            db.HashSet(settings.Redis.UserKey, userEntries);
            db.HashSet(settings.Redis.SubredditSubscriptionKey, subscriptions);
        }
    }
}
