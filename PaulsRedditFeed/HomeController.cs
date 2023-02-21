using Microsoft.AspNetCore.Mvc;

namespace PaulsRedditFeed
{
    public class HomeController : Controller
    {
        [HttpPut("/subreddits")]
        public async Task<IActionResult> Subreddits([FromBody] List<SubredditViewModel> model)
        {
            return PartialView("_SubredditList", model);
        }
    }
}
