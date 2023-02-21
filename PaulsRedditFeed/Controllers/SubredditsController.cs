using Microsoft.AspNetCore.Mvc;

namespace PaulsRedditFeed
{
    [Route("subreddits")]
    public class SubredditsController : Controller
    {
        [HttpPut("")]
        public IActionResult Index([FromBody] List<SubredditViewModel> model)
        {
            return PartialView("_SubredditList", model);
        }
    }
}
