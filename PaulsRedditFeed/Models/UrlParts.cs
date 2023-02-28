using Microsoft.AspNetCore.WebUtilities;

namespace PaulsRedditFeed.Models;

public class UrlParts
{
    public string Path { get; set; } = string.Empty;
    public string PathAndQuery { get; set; } = string.Empty;
    public string QueryString { get; set; } = string.Empty;

    public UrlParts() { }

    public UrlParts(string url)
    {
        PathAndQuery = url.Substring(url.Trim().Trim('\\', '/').Trim().IndexOf("r/"));
        var queryStart = PathAndQuery.IndexOf('?');
        if (queryStart > -1)
        {
            QueryString = PathAndQuery.Substring(queryStart);
            Path = PathAndQuery.Substring(0, queryStart);
        }
        else
        {
            Path = PathAndQuery;
        }
    }
}
