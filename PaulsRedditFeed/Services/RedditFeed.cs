using Reddit;
using Reddit.Controllers;
using Reddit.Inputs.Search;

namespace PaulsRedditFeed
{
    public class RedditFeed : BackgroundService
    {
        private readonly ILogger<RedditFeed> logger;
        private readonly RedditClient reddit;

        public RedditFeed(ILogger<RedditFeed> logger, RedditClient reddit)
        {
            this.logger = logger;
            this.reddit = reddit;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation($"${nameof(RedditFeed)} Background Service Started");
            while (!stoppingToken.IsCancellationRequested)
            {
                // TODO find all subreddits in db and long poll them
                var subreddit = await Task.Run(() => reddit.Subreddit("AskReddit").About());
                var hottestPost = subreddit.Posts.GetHot(limit: 1).OrderByDescending(post => post.Score).First();
            }
            logger.LogInformation($"${nameof(RedditFeed)} Background Service Stopped");
        }

        private async Task ReadStreamAsync()
        {
            logger.LogInformation("Reading filtered Reddit stream");
            await Task.Delay(1000);
        }
    }
}
