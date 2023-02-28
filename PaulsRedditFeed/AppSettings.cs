namespace PaulsRedditFeed
{
    public class AppSettings
    {
        public RedditSettings Reddit { get; set; } = new RedditSettings();
        public RedisSettings Redis { get; set; } = new RedisSettings();
        public int PollingIntervalMilliseconds { get; set; } = 1000;
    }

    public class RedditSettings
    {
        public string AppId { get; set; } = string.Empty;
        public string AppSecret { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public string UserAgent { get; set; } = string.Empty;

        /// <summary>
        /// The base url for the reddit API to use for collecting monitoring data
        /// </summary>
        public string BaseUrl { get; set; } = string.Empty;

        /// <summary>
        /// The base url for the actual reddit API
        /// </summary>
        public string LiveBaseUrl { get; set; } = string.Empty;

        public string AuthUrl { get; set; } = string.Empty;

        /// <summary>
        /// The filters a new user gets if they haven't chose any
        /// </summary>
        public UserFilters DefaultFilters { get; set; } = new UserFilters();
    }

    public class RedisSettings
    {
        public string ConnectionString { get; set; } = string.Empty;

        /// <summary>
        /// Name of the pub sub channel for unprocessed subreddit data
        /// </summary>
        public string QueueChannelName { get; } = "subreddit_raw_data";

        /// <summary>
        /// Key for users hash
        /// </summary>
        public string UserKey { get; } = "users";

        /// <summary>
        /// Key for hash to cached subreddit info data
        /// </summary>
        public string SubredditInfoKey { get; } = "subreddit_info";

        /// <summary>
        /// Key for hash to cached hot post data
        /// </summary>
        public string HotPostInfoKey { get; } = "hotpost_info";

        /// <summary>
        /// Key for redis hash that stores the state of monitored subreddits
        /// </summary>
        public string SubredditSubscriptionKey { get; } = "subreddit_subscriptions";

        /// <summary>
        /// Key for redis hash that stores the state of monitored subreddits
        /// </summary>
        public string MonitorQueueKey { get; } = "subreddit_monitor_queue";

        /// <summary>
        /// Key for distributed locking so all the server instances don't do the same work
        /// </summary>
        public string MonitorQueueLock { get; } = "subreddit_monitor_queue_lock";
    }
}
