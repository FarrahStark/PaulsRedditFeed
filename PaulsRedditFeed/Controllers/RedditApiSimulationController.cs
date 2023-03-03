using Azure.Core;
using Microsoft.AspNetCore.Mvc;

namespace PaulsRedditFeed;

[AuthorizeAsReddit]
[Route("fakereddit")]
public class RedditApiSimulationController : Controller
{
    private readonly ILogger<RedditApiSimulationController> logger;
    private readonly RedditApiClient reddit;

    public RedditApiSimulationController(
        ILogger<RedditApiSimulationController> logger,
        RedditApiClient reddit)
    {
        this.logger = logger;
        this.reddit = reddit;
    }

    [HttpGet("r/{subreddit}/hot")]
    public async Task<IActionResult> Hot(string subreddit)
    {
        var request = Request.ToUrlParts();
        if (string.IsNullOrWhiteSpace(request.QueryString))
        {
            // Set default search filters query string if we weren't passed any
            request.QueryString = "?g=&show=all&sr_detail=False&after=&before=&limit=5&count=0&raw_json=1";
            request.PathAndQuery = request.Path + request.QueryString;
        }

        string json = await reddit.SendRequestAsync<HotPostRawData>(request);
        return Ok(json);
    }

    [HttpGet("r/{subreddit}/about")]
    public async Task<IActionResult> About(string subreddit)
    {
        string json = await reddit.SendRequestAsync<RawSubredditInfo>(Request.ToUrlParts());
        return Ok(json);
    }
}
