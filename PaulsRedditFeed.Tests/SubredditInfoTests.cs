using Microsoft.Extensions.DependencyInjection;
using System.Net;

namespace PaulsRedditFeed.Tests
{
    public class SubredditInfoTests : IntegrationTest
    {
        const string Token = "Afaketoken124";

        [Theory]
        [InlineData($"bearer {Token}", HttpStatusCode.OK)]
        [InlineData($"bearer saldkfjlksajf", HttpStatusCode.Forbidden)]
        [InlineData($"aklfujlksafj098324", HttpStatusCode.Forbidden)]
        public async Task Request_rejected_when_not_authorized(string authHeaderValue, HttpStatusCode expectedStatus)
        {
            var request = CreateRequest(r =>
            {
                r.RequestUri = new Uri("fakereddit/r/Music/hot", UriKind.Relative);
                r.Headers.Add("Authorization", authHeaderValue);
            });

            var response = await HttpClient.SendAsync(request);
            response.StatusCode.Should().Be(expectedStatus, "Only valid auth headers allow a successful response");
        }

        private HttpRequestMessage CreateRequest(Action<HttpRequestMessage> configure = null)
        {
            var settings = Services.GetRequiredService<AppSettings>();
            var request = new HttpRequestMessage()
            {
                Method = HttpMethod.Get
            };

            request.Headers.Add("Accept", "application/json, text/json, text/x-json, text/javascript, application/xml, text/xml");
            request.Headers.Add("userAgent", settings.Reddit.UserAgent);
            request.Headers.Add("Connection", "Kepp-Alive");
            request.Headers.Add("Accept-Encoding", "gzip, deflate");

            if (configure != null)
            {
                configure(request);
            }

            return request;
        }

        protected override void ConfigureServices(IServiceCollection services)
        {
            var settings = services.BuildServiceProvider().GetRequiredService<AppSettings>();
            var settingsDescriptor = services.SingleOrDefault(services => services.ServiceType == typeof(AppSettings));
            if (settingsDescriptor != null)
            {
                services.Remove(settingsDescriptor);
            }

            // Spoof the refresh token for this test class
            settings.Reddit.RefreshToken = Token;
            services.AddSingleton(settings);
        }
    }
}
