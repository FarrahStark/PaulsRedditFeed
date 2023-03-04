using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Net;

namespace PaulsRedditFeed;

/// <summary>
/// Renders the main and error pages
/// </summary>
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }


    /// <summary>
    /// The health check endpoint.
    /// When testing integrations with this health check the caller can simulate
    /// different failures using the <paramref name="testFail"/> query string parameter
    /// </summary>
    /// <param name="testFail">When <paramref name="testFail"/> is passed
    /// the health check fails with a Service Unavailable (503) status code.
    /// Used for testing integrations with this health check
    /// </param>
    /// <returns>a string indicating the health status of the server.
    /// 200 responses indicate a healthy server</returns>
    [HttpGet("/health")]
    public IActionResult Health([FromQuery] string testFail = "")
    {
        if (!string.IsNullOrWhiteSpace(testFail))
        {
            return StatusCode((int)HttpStatusCode.ServiceUnavailable, testFail);
        }

        return Ok("healthy");
    }

    /// <summary>
    /// Endpoint to return the home page
    /// </summary>
    public IActionResult Index()
    {
        return View();
    }

    /// <summary>
    /// Endpoint to return the privacy page
    /// </summary>
    public IActionResult Privacy()
    {
        return View();
    }

    /// <summary>
    /// Endpoint to display the default error page
    /// </summary>
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}