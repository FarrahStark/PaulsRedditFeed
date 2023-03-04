using Microsoft.AspNetCore.Mvc;

namespace PaulsRedditFeed;

/// <summary>
/// renders server side views for data updates
/// </summary>
[Route("subreddits")]
public class SubredditsController : Controller
{
    private readonly ILogger<SubredditsController> logger;

    public SubredditsController(ILogger<SubredditsController> logger)
    {
        this.logger = logger;
    }

    /// <summary>
    /// Renders a list of subreddit stats with the data in <paramref name="model"/>
    /// </summary>
    /// <param name="model">the stats to render</param>
    /// <returns>a rendered view of the stats</returns>
    [HttpPut("")]
    public IActionResult Index([FromBody] List<SubredditStatsViewModel> model)
    {
        logger.LogDebug("Returning rerendered subreddit stats");
        return PartialView("_SubredditList", model);
    }
}
