using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using StackExchange.Redis;

namespace PaulsRedditFeed.Controllers
{
    [AuthorizeAsReddit]
    [Route("fakereddit")]
    public class RedditApiSimulationController : Controller
    {
        private static readonly Random random = new Random();
        private readonly IHttpClientFactory httpClientFactory;
        private readonly AppSettings settings;
        private readonly ConnectionMultiplexer redis;

        private HttpClient redditApi => httpClientFactory.CreateClient(RedditTokenHandler.SearchClientName);

        public RedditApiSimulationController(
            ILogger<RedditApiSimulationController> logger,
            AppSettings settings,
            ConnectionMultiplexer redis,
            IHttpClientFactory httpClientFactory)
        {
            this.settings = settings;
            this.redis = redis;
            this.httpClientFactory = httpClientFactory;
        }

        [HttpGet("r/{subreddit}/hot")]
        public async Task<IActionResult> Hot(string subreddit)
        {
            var query = Request.QueryString;
            var url = $"/r/{subreddit}/hot{query}";
            string json = await SendRedditLiveApiRequest<HotPostRawData>(url);
            return Json(json);
        }

        [HttpGet("r/{subreddit}/about")]
        public async Task<IActionResult> About(string subreddit)
        {
            var query = Request.QueryString;
            var url = $"/r/{subreddit}/about{query}";
            string json = await SendRedditLiveApiRequest<RawSubredditInfo>(url);
            return Json(json);
        }

        private async Task<string> SendRedditLiveApiRequest<TModel>(string url) where TModel : new()
        {
            string hashKey;
            var modelName = typeof(TModel).Name;
            switch (modelName)
            {
                case nameof(HotPostRawData):
                    hashKey = settings.Redis.HotPostInfoKey;
                    break;
                case nameof(RawSubredditInfo):
                    hashKey = settings.Redis.HotPostInfoKey;
                    break;
                default: throw new NotImplementedException($"No reddit request cache has been configured for {typeof(TModel).Name}");
            }

            var redisDb = redis.GetDatabase();
            var cachedResult = await redisDb.HashGetAsync(hashKey, url);

            string json = "{}";
            if (!cachedResult.HasValue || cachedResult.IsNullOrEmpty)
            {
                var requestUrl = settings.Reddit.LiveBaseUrl + url;
                var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);
                var redditResponse = await redditApi.SendAsync(request);
                json = await redditResponse.Content.ReadAsStringAsync();
                await redisDb.HashSetAsync(settings.Redis.SubredditInfoKey, new[] { new HashEntry(url, json) });
            }
            else
            {
                json = cachedResult.ToString();
            }

            // Randomize data to make it look like real time API updates
            switch (modelName)
            {
                case nameof(HotPostRawData):
                    var hotPost = JsonSerializer.Deserialize<HotPostRawData>(json);
                    json = JsonSerializer.Serialize(hotPost);
                    break;
                case nameof(RawSubredditInfo):
                    var aboutSubreddit = JsonSerializer.Deserialize<RawSubredditInfo>(json);
                    if (aboutSubreddit == null)
                    {
                        throw new JsonSerializationException($"unable to deserialize json to a {nameof(SubredditRawData)}");
                    }

                    var userFluctuationRatio = random.NextDouble() * 0.5 * (random.Next(0, 2) == 0 ? -1 : 1);
                    var userCount = aboutSubreddit.data.active_user_count;
                    var simulatedUserCount = userCount + (int)(userCount * userFluctuationRatio);
                    aboutSubreddit.data.active_user_count = simulatedUserCount;
                    json = JsonSerializer.Serialize(aboutSubreddit);
                    break;
                default: throw new NotImplementedException($"No reddit request cache has been configured for {typeof(TModel).Name}");
            }
            return json ?? throw new CachingException($"Cache lookup for request failed {url}");
        }
    }
}
