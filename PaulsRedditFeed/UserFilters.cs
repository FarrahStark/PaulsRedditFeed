namespace PaulsRedditFeed
{
    public class UserFilters
    {
        public string TimeFrame = "day";
        public string Listing = "best";
        public uint Limit = 1;
        public string[] StartingSubreddits = Array.Empty<string>();
    }
}
