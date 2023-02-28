using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.WebUtilities;
using PaulsRedditFeed.Controllers;
using PaulsRedditFeed.Models;
using Reddit;
using StackExchange.Redis;
using System;
using System.Text.Json;

namespace PaulsRedditFeed;

public class RedditApiClient
{
    private static readonly Random random = new Random();
    private readonly AppSettings settings;
    private readonly ConnectionMultiplexer redis;
    private HttpClient redditApi => httpClientFactory.CreateClient(RedditTokenHandler.SearchClientName);
    private readonly IHttpClientFactory httpClientFactory;
    private readonly ILogger<RedditApiClient> logger;

    public RedditApiClient(
        ILogger<RedditApiClient> logger,
            AppSettings settings,
            ConnectionMultiplexer redis,
            IHttpClientFactory httpClientFactory)
    {
        this.logger = logger;
        this.settings = settings;
        this.redis = redis;
        this.httpClientFactory = httpClientFactory;
    }

    public async Task<string> SendRequestAsync<TModel>(HttpRequest request) where TModel : new()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Gets a cached response or hits the reddit api and caches the response
    /// </summary>
    /// <typeparam name="TModel">The model for the reddit api dto</typeparam>
    /// <param name="url">The url to send the request to</param>
    /// <returns>a json string representing the data returned from reddit</returns>
    /// <exception cref="NotImplementedException"></exception>
    /// <exception cref="JsonSerializationException"></exception>
    /// <exception cref="CachingException"></exception>
    public async Task<string> SendRequestAsync<TModel>(UrlParts request) where TModel : new()
    {
        int targetLimit = 1;
        var queryValues = QueryHelpers.ParseQuery(request.QueryString);

        int requestedLimit = 5;
        if (queryValues.ContainsKey("limit") && int.TryParse(queryValues["limit"], out int parsedLimit))
        {
            requestedLimit = parsedLimit;
        }

        string responseCacheKey;
        var modelName = typeof(TModel).Name;
        switch (modelName)
        {
            case nameof(HotPostRawData):
                responseCacheKey = settings.Redis.HotPostInfoKey;
                targetLimit = 10;
                break;
            case nameof(RawSubredditInfo):
                responseCacheKey = settings.Redis.SubredditInfoKey;
                targetLimit = 1;
                break;
            default: throw new NotImplementedException($"No reddit request cache has been configured for {typeof(TModel).Name}");
        }

        responseCacheKey += $"_{request.PathAndQuery}";
        var cacheLifespan = TimeSpan.FromSeconds(30);
        var calculatedLimit = Math.Max(requestedLimit, targetLimit).ToString();
        queryValues["limit"] = calculatedLimit;

        var redisDb = redis.GetDatabase();
        var cachedResult = await redisDb.StringGetAsync(responseCacheKey);

        string json = "{}";
        if (!cachedResult.HasValue || cachedResult.IsNullOrEmpty)
        {
            var baseUri = new Uri(settings.Reddit.LiveBaseUrl);
            var requestUri = new Uri(baseUri, request.Path).ToString();
            var requestUrl = QueryHelpers.AddQueryString(requestUri, queryValues);
            var redditRequest = new HttpRequestMessage(HttpMethod.Get, requestUrl);
            var redditResponse = await redditApi.SendAsync(redditRequest);
            json = await redditResponse.Content.ReadAsStringAsync();

            logger.LogDebug($"No cached result found. Querying redditAPI at {request.PathAndQuery}");
            await redisDb.StringSetAsync(responseCacheKey, json, cacheLifespan);
        }
        else
        {
            logger.LogDebug($"Returning cached result for {request.PathAndQuery}");
            json = cachedResult.ToString();
        }

        var serializerOptions = new JsonSerializerOptions()
        {
            IgnoreReadOnlyFields = true,
            IgnoreReadOnlyProperties = true,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        };

        // Randomize data to make it look like real time API updates
        switch (modelName)
        {
            case nameof(HotPostRawData):
                try
                {
                    var hotPost = JsonSerializer.Deserialize<HotPostRawData>(json, serializerOptions);
                    if (hotPost == null)
                    {
                        throw new JsonSerializationException($"unable to deserialize json to a {nameof(HotPostRawData)}");
                    }

                    hotPost.data.children = hotPost.data.children.Select(post =>
                    {
                        post.data.ups = Math.Clamp(GetFluctuatedValue(post.data.ups), 0, int.MaxValue);
                        var ratio = post.data.upvote_ratio <= 0 ? 1 : post.data.upvote_ratio;
                        post.data.downs = (int)(post.data.ups / ratio);
                        return post;
                    }).OrderByDescending(post => post.data.upvote_ratio).ToArray();
                    json = JsonSerializer.Serialize(hotPost, serializerOptions);
                }
                catch (Exception ex)
                {

                }
                break;
            case nameof(RawSubredditInfo):
                var aboutSubreddit = JsonSerializer.Deserialize<RawSubredditInfo>(json, serializerOptions);
                if (aboutSubreddit == null)
                {
                    throw new JsonSerializationException($"unable to deserialize json to a {nameof(SubredditRawData)}");
                }

                aboutSubreddit.data.active_user_count = GetFluctuatedValue(aboutSubreddit.data.active_user_count);
                json = JsonSerializer.Serialize(aboutSubreddit, serializerOptions);
                break;
            default: throw new NotImplementedException($"No reddit request cache has been configured for {typeof(TModel).Name}");
        }

        return json ?? throw new CachingException($"Cache lookup for request failed {request.PathAndQuery}");
    }

    private static int GetFluctuatedValue(int value)
    {
        var fluctuationRatio = (random.NextDouble() * (random.Next(0, 2) == 0 ? -1 : 1)) / 2;
        return value + (int)(value * fluctuationRatio);
    }
}
