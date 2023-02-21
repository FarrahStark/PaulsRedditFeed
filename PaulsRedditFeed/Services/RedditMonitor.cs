using Reddit;
using Reddit.Controllers;
using Reddit.Inputs.Search;

namespace PaulsRedditFeed
{
    public class RedditMonitor : BackgroundService
    {
        private readonly ILogger<RedditMonitor> logger;
        private readonly RedditClient reddit;

        public RedditMonitor(ILogger<RedditMonitor> logger, RedditClient reddit)
        {
            this.logger = logger;
            this.reddit = reddit;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            //logger.LogInformation($"${nameof(RedditMonitor)} RedditMonitor Started");
            //while (!stoppingToken.IsCancellationRequested)
            //{
            //    try
            //    {
            //        logger.LogDebug("Starting subreddit scan");
            //        // TODO find all subreddits in db and long poll them
            //        var subreddit = await Task.Run(() => reddit.Subreddit("AskReddit").About());
            //        var hottestPost = subreddit.Posts.GetHot(limit: 1).OrderByDescending(post => post.Score).First();
            //        await Task.Delay(1000);
            //    }
            //    catch (Exception ex)
            //    {

            //    }
            //}
            //logger.LogInformation($"${nameof(RedditMonitor)} Background Service Stopped");
        }
    }
}
