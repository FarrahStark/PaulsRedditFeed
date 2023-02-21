namespace PaulsRedditFeed
{
    public class AppSettings
    {
        public RedditSettings Reddit { get; set; } = new RedditSettings();

        public class RedditSettings
        {
            public string AppId { get; set; } = string.Empty;
            public string ApiKey { get; set; } = string.Empty;
            public string RefreshToken { get; set; } = string.Empty;
            public string ModHash { get; set; } = string.Empty;
            public string Url { get; set; } = string.Empty;
            public UserFilters DefaultFilters { get; set; } = new UserFilters();
        }
    }
}
