using Microsoft.AspNetCore.Mvc;

namespace PaulsRedditFeed.Controllers
{
    [AuthorizeAsReddit]
    [Route("fakereddit/r/")]
    public class RedditApiSimulationController : Controller
    {
        [HttpGet("{subreddit}/hot")]
        public IActionResult Hot()
        {
            // action simulates this get request
            // https://oauth.reddit.com/r/DIY/hot?g=&show=all&over_18=no&sr_detail=False&after=&before=&limit=1&count=0&raw_json=1
            return Json(new { Prop = "this is a response" });
        }

        [HttpGet("/about")]
        public IActionResult About()
        {
            // action simulates this get request
            // https://oauth.reddit.com/r/Baking/about%20HTTP/1.1
            throw new NotImplementedException();
        }
    }
}
