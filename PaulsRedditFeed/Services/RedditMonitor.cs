using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Distributed;
using Reddit;
using Reddit.Controllers;
using Reddit.Inputs.Search;
using Reddit.Models;
using Reddit.Things;
using System.Text.Json;

namespace PaulsRedditFeed
{
    public class RedditMonitor : BackgroundService
    {
        private static readonly Random random = new Random();
        private readonly ILogger<RedditMonitor> logger;
        private readonly RedditClient reddit;
        private readonly IDistributedCache cache;
        private readonly RedditSettings redditSettings;
        private readonly IHubContext<RedditStatsHub> statsHub;
        private static readonly string MonitoredSubreddits = "monitored_subreddits";
        private static readonly string users = "users";

        public RedditMonitor(
            ILogger<RedditMonitor> logger,
            RedditClient reddit,
            IDistributedCache cache,
            RedditSettings redditSettings,
            IHubContext<RedditStatsHub> statsHub)
        {
            this.logger = logger;
            this.reddit = reddit;
            this.cache = cache;
            this.redditSettings = redditSettings;
            this.statsHub = statsHub;
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

                    var getValue = () => random.Next(0, 9999);
                    var stats = new List<object>()
                        {
                            new { Title= "Subreddit1", TopPostTitle= "Whoa dude!", ActiveUserCount = getValue() },
                            new { Title= "Subreddit2", TopPostTitle= "Candy!", ActiveUserCount = getValue() },
                            new { Title= "Subreddit3", TopPostTitle= "Cat Tax", ActiveUserCount = getValue() },
                        };

                    await NotifyClientsAsync(stats);
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

        public async Task NotifyClientsAsync<T>(IEnumerable<T> stats)
        {
            await statsHub.Clients.All.SendAsync("ReceiveMessage", JsonSerializer.Serialize(stats));
        }
    }
}
