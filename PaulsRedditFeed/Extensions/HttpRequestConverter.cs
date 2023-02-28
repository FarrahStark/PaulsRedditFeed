using Microsoft.AspNetCore.Http.Extensions;
using PaulsRedditFeed.Models;

namespace PaulsRedditFeed;

public static class HttpRequestConverter
{
    public static UrlParts ToUrlParts(this HttpRequest request)
    {
        var uri = new Uri(request.GetEncodedUrl(), UriKind.Absolute);
        var url = "/" + uri.LocalPath.Replace("fakereddit", "").Trim().Trim('\\', '/').Trim();

        return new UrlParts
        {
            Path = url,
            PathAndQuery = url + request.QueryString.Value,
            QueryString = request.QueryString.Value ?? "",
        };
    }
}
