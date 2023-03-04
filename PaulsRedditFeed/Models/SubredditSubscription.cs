namespace PaulsRedditFeed
{
    /// <summary>
    /// Used to keep track of active users watching a particular subreddit so we don't
    /// monitor subreddits that nobody is watching
    /// </summary>
    public class SubredditSubscription
    {
        public string Subreddit { get; set; } = string.Empty;
        public int SubscriberCount { get; set; } = 0;
    }
}
