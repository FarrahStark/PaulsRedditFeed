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
    [Produces("application/json")]
    public async Task<IActionResult> Hot(string subreddit)
    {
        string json = await reddit.SendRequestAsync<HotPostRawData>(Request.ToUrlParts());
        return Ok(json);
    }

    [HttpGet("r/{subreddit}/about")]
    [Produces("application/json")]
    public async Task<IActionResult> About(string subreddit)
    {
        string json = await reddit.SendRequestAsync<RawSubredditInfo>(Request.ToUrlParts());
        return Ok(json);
    }
}
