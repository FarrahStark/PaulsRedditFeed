using System.Net;

namespace PaulsRedditFeed.Tests;

/// <summary>
/// Tests for the /health endpoint used for healthchecks to the application
/// </summary>
public class HealthCheckTests : RedditFeedIntegrationTest
{
    [Theory]
    [InlineData(HttpStatusCode.OK, "healthy")]
    [InlineData(HttpStatusCode.ServiceUnavailable, "science rules", "?testfail=science%20rules")]
    [InlineData(HttpStatusCode.ServiceUnavailable, "DB Unavailable", "?testfail=DB%20Unavailable")]
    public async Task Server_responds_with_healthy_ok_responses(
        HttpStatusCode expectedStatus,
        string expectedHealthStatus,
        string queryString = "")
    {
        var healthcheckResponse = await HttpClient.GetAsync($"/health{queryString}");
        var healthStatus = await healthcheckResponse.Content.ReadAsStringAsync();
        healthStatus.Should().Be(expectedHealthStatus,
            "The response content must be a status string that indicates the health of the server");
        healthcheckResponse.StatusCode.Should().Be(expectedStatus,
            "The Http response code must accurately indicate the health of the server");
    }
}
