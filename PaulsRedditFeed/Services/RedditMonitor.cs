namespace PaulsRedditFeed
{
    public class RedditMonitor : BackgroundService
    {
        /// <summary>
        /// A unique id for each running instance of PaulsRedditFeed.
        /// </summary>
        private static readonly string ServerInstanceId = Guid.NewGuid().ToString();
        private static readonly Random random = new Random();
        private readonly ILogger<RedditMonitor> logger;
        private readonly RedditApiClient reddit;
        private readonly ConnectionMultiplexer redis;
        private readonly AppSettings settings;
        private ISubscriber messageQueue;

        public RedditMonitor(
            ILogger<RedditMonitor> logger,
            RedditApiClient reddit,
            ConnectionMultiplexer redis,
            AppSettings settings)
        {
            this.logger = logger;
            this.reddit = reddit;
            this.redis = redis;
            this.settings = settings;
            messageQueue = redis.GetSubscriber();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            SeedCache();
            logger.LogInformation($"{nameof(RedditMonitor)} Started");
            try
            {
                await StartMonitoringSubreddits(stoppingToken);
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

        /// <summary>
        /// Fetches updated data for each actively monitored subreddit and queues it for processing.
        /// Uses the TPL for efficient thread usage instead of trying to manage the threads
        /// manually.
        /// </summary>
        /// <param name="stoppingToken">Allows the async operations to be cancelled</param>
        /// <returns>a task tracking the async operation</returns>
        private async Task StartMonitoringSubreddits(CancellationToken stoppingToken)
        {
            // Get updated info about monitored subreddits
            var subredditSubscriptions = await redis.GetDatabase()
                .HashGetAllAsync(settings.Redis.SubredditSubscriptionKey);

            var db = redis.GetDatabase();

            // One of the server instances will get a distributed lock here, and will get the monitoring queue running
            if (await db.LockTakeAsync(settings.Redis.MonitorQueueLock, ServerInstanceId, TimeSpan.FromSeconds(30)))
            {
                try
                {
                    logger.LogInformation("Monitor queue lock acquired. Initializing monitoring queue tasks");
                    var monitoredSubreddits = subredditSubscriptions
                        .Where(s => int.Parse(s.Value) > 0)
                        .Select(s => (string)s.Name)
                        .OrderBy(subreddit => subreddit, StringComparer.InvariantCultureIgnoreCase)
                        .ToArray();

                    foreach (var subredditName in monitoredSubreddits)
                    {
                        db.ListRightPush(settings.Redis.MonitorQueueKey, subredditName);
                    }
                }
                finally
                {
                    logger.LogInformation("Monitor queue initialized. Releasing lock");
                    await db.LockReleaseAsync(settings.Redis.MonitorQueueKey, ServerInstanceId);
                }
            }
            else
            {
                logger.LogInformation("Unable to get lock. Monitor queue is probably being initialzed by a different server instance");
            }

            var monitorIntervalMs = 3000;
            var startMonitorAfter = random.Next(1, monitorIntervalMs);
            await Task.Delay(startMonitorAfter); // Staggers the monitoring per server so they don't all collect at the exact same time

            // watch the queue for incoming tasks
            while (!stoppingToken.IsCancellationRequested)
            {
                // subreddits are monitored in round robin by servers. Pop will get the next item.
                var subredditName = await db.ListLeftPopAsync(settings.Redis.MonitorQueueKey);
                try
                {
                    logger.LogDebug($"Scanning {subredditName} for updates");
                    await MonitorSubreddit(subredditName, stoppingToken);
                }
                finally
                {
                    // Once a subreddit has been monitored, wait a bit and throw it back on the queue
                    // to get that subreddit monitored again by one of the server instances
                    await Task.Delay(3000);
                    await db.ListRightPushAsync(settings.Redis.MonitorQueueKey, subredditName);
                }
            }
        }

        private async Task MonitorSubreddit(string subredditName, CancellationToken stoppingToken)
        {
            try
            {
                logger.LogInformation($"Scanning subreddit r/{subredditName}");
                HotPostData hotPosts;
                SubredditRawData subredditData;

                // Collect json data from reddit on a single subreddit
                var hotRequest = reddit.SendRequestAsync<HotPostRawData>(
                    $"r/{subredditName}/hot?g=&show=all&sr_detail=False&after=&before=&limit=5&count=0&raw_json=1");
                var aboutRequest = reddit.SendRequestAsync<SubredditRawData>(
                    $"r/{subredditName}/about");

                await Task.WhenAll(new[] { aboutRequest, hotRequest });

                // pass returned json from reddit straight to the queue without deserializing
                var dataJson = $"{{\"HotPosts\": {hotRequest.Result},\"Subreddit\": {aboutRequest.Result}}}";
                // Queue up the collected data for processing
                await messageQueue.PublishAsync(settings.Redis.QueueChannelName, dataJson);
                logger.LogInformation($"Scan complete r/{subredditName}");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Scanning of subreddit {subredditName} failed.");
            }
            finally
            {
                // queue up the subreddit to get monitored again later
                await messageQueue.PublishAsync(settings.Redis.MonitorQueueKey, subredditName);
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

        public override void Dispose()
        {
            messageQueue.Unsubscribe(settings.Redis.MonitorQueueKey);
            base.Dispose();
        }
    }
}
