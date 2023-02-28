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
        public string BaseUrl { get; set; } = string.Empty;

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
        /// Key for redis hash that stores the state of monitored subreddits
        /// </summary>
        public string SubredditSubscriptionKey { get; } = "subreddit_subscriptions";
    }
}
