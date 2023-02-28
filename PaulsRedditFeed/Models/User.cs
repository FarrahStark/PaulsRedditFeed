namespace PaulsRedditFeed
{
    public class User
    {
        public int Id { get; set; } = -1;
        public List<string> SubscribedSubreddits { get; set; } = new List<string>();
    }
}
