namespace PaulsRedditFeed
{
    /// <summary>
    /// View model to show stats about subreddits to users
    /// </summary>
    public class SubredditStatsViewModel
    {
        public string Title { get; set; } = "";
        public int ActiveUserCount { get; set; } = 0;
        public string TopPostTitle { get; set; } = "";
        public int TopPostUpvotes { get; set; } = 0;
        public int TopPostDownvotes { get; set; } = 0;
    }
}
