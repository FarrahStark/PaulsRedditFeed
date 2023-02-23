using Microsoft.Extensions.Caching.Distributed;
using Reddit;
using Reddit.Controllers;
using Reddit.Inputs.Search;

namespace PaulsRedditFeed
{
    public class RedditMonitor : BackgroundService
    {
        private readonly ILogger<RedditMonitor> logger;
        private readonly RedditClient reddit;
        private readonly IDistributedCache cache;
        private readonly RedditSettings redditSettings;
        private static readonly string MonitoredSubreddits = "monitored_subreddits";
        private static readonly string users = "users";

        public RedditMonitor(
            ILogger<RedditMonitor> logger,
            RedditClient reddit,
            IDistributedCache cache,
            RedditSettings redditSettings)
        {
            this.logger = logger;
            this.reddit = reddit;
            this.cache = cache;
            this.redditSettings = redditSettings;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation($"${nameof(RedditMonitor)} Started");
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    // TODO find all subreddits in db and long poll them
                    logger.LogDebug("Starting subreddit scan");

                    var subreddits = await cache.GetAsync<string[]>($"{MonitoredSubreddits}_u0", stoppingToken);





                    var subreddit = await Task.Run(() => reddit.Subreddit("AskReddit").About());



                    var hottestPost = subreddit.Posts.GetHot(limit: 1).OrderByDescending(post => post.Score).First();
                    logger.LogDebug("subreddit scan completed");
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Subreddit scan failed");
                }
                finally
                {
                    await Task.Delay(1000);
                }
            }
            logger.LogInformation($"${nameof(RedditMonitor)} Stopped");
        }
    }
}
