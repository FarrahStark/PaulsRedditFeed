using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using StackExchange.Redis;
using Microsoft.AspNetCore.WebUtilities;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http.Extensions;

namespace PaulsRedditFeed.Controllers
{
    [AuthorizeAsReddit]
    [Route("fakereddit")]
    public class RedditApiSimulationController : Controller
    {

        private readonly IHttpClientFactory httpClientFactory;
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
            string json = await reddit.SendRequestAsync<HotPostRawData>(Request.ToUrlParts());
            return Json(json);
        }

        [HttpGet("r/{subreddit}/about")]
        public async Task<IActionResult> About(string subreddit)
        {
            string json = await reddit.SendRequestAsync<RawSubredditInfo>(Request.ToUrlParts());
            return Json(json);
        }
    }
}
