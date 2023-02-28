namespace PaulsRedditFeed;

public static class RedditClientExtensions
{
    public const string ClientName = "RedditHttpClient";

    public static HttpClient GetRedditClient(this IHttpClientFactory factory)
    {
        return factory.CreateClient(ClientName);
    }
}
