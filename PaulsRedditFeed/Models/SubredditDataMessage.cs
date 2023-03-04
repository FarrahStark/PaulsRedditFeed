namespace PaulsRedditFeed;

/// <summary>
/// Unprocessed redit response data
/// </summary>
public class SubredditDataMessage
{
    public SubredditRawData Subreddit { get; set; }
    public HotPostRawData HotPosts { get; set; }
}
