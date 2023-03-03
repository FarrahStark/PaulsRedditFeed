using IdentityModel.Client;

namespace PaulsRedditFeed;

/// <summary>
/// Ensures reqeuests to the reddit api have an unexpired access token.
/// Requests an OAuth2 access token from the reddit API for authenticatcation
/// of subsequent requests
/// </summary>
public class RedditTokenHandler : DelegatingHandler
{
    public const string AuthClientName = "RedditAuth";
    public const string SearchClientName = "RedditSearch";
    private readonly ILogger<RedditTokenHandler> logger;
    private readonly IHttpClientFactory httpClientFactory;
    private readonly AppSettings settings;
    private HttpClient AuthClient => httpClientFactory.CreateClient(AuthClientName);

    public RedditTokenHandler(
        ILogger<RedditTokenHandler> logger,
        IHttpClientFactory httpClientFactory,
        AppSettings settings)
    {
        this.logger = logger;
        this.httpClientFactory = httpClientFactory;
        this.settings = settings;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        // request the access token
        var accessToken = await AuthClient.RequestClientCredentialsTokenAsync(
            new ClientCredentialsTokenRequest
            {
                Address = settings.Reddit.AuthUrl,
                ClientId = settings.Reddit.AppId,
                ClientSecret = settings.Reddit.AppSecret,
                Scope = "*"
            });

        if (accessToken.IsError)
        {
            logger.LogError(accessToken.Error);
            throw new HttpRequestException("Something went wrong while requesting the access token");
        }
        else
        {
            request.Headers.Add("Authorization", $"Bearer {accessToken.AccessToken}");
        }

        // Proceed calling the inner handler, that will actually send the request
        return await base.SendAsync(request, cancellationToken);
    }
}
