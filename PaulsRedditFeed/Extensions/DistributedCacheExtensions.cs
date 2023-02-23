using Microsoft.Extensions.Caching.Distributed;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.Json;

namespace PaulsRedditFeed
{
    /// <summary>
    /// Makes it easier to work with caching objects as binary data when needed
    /// </summary>
    public static class DistributedCacheExtensions
    {
        /// <summary>
        /// Serializes the <paramref name="value"/> and caches it using the <paramref name="key"/>
        /// </summary>
        /// <typeparam name="T">The type of object to cache</typeparam>
        /// <param name="distributedCache">The cache to set stuff in</param>
        /// <param name="key">the key used to store and retrieve the cached value</param>
        /// <param name="value">the object to store</param>
        /// <param name="options">the distributed caching options</param>
        /// <param name="token">stops caching if the operation is cancelled</param>
        /// <returns>a task tracking the caching operation</returns>
        public async static Task SetAsync<T>(this IDistributedCache distributedCache, string key, T value, DistributedCacheEntryOptions options, CancellationToken token = default(CancellationToken)) where T : class
        {
            await distributedCache.SetStringAsync(key, JsonSerializer.Serialize(value), options, token);
        }

        /// <summary>
        /// Retreives a cached value using <paramref name="key"/> and deserializes into a <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T">the type of object to retrieve from the cache</typeparam>
        /// <param name="distributedCache">The cache to set stuff in</param>
        /// <param name="key">the key used to store and retrieve the cached value</param>
        /// <param name="token">stops caching if the operation is cancelled</param>
        /// <returns>a task tracking the caching operation</returns>
        public async static Task<T?> GetAsync<T>(this IDistributedCache distributedCache, string key, CancellationToken token = default(CancellationToken)) where T : class
        {
            var valueJson = await distributedCache.GetStringAsync(key, token);
            if (valueJson == null)
            {
                return null;
            }

            return JsonSerializer.Deserialize<T>(valueJson);
        }
    }
}
