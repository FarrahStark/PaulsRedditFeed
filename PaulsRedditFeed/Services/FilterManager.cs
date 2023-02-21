using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace PaulsRedditFeed
{
    public class FilterManager
    {
        private readonly IDistributedCache cache;
        private readonly AppSettings settings;

        public FilterManager(IDistributedCache cache, AppSettings settings)
        {
            this.cache = cache;
            this.settings = settings;
        }

        /// <summary>
        /// Caches the passed <paramref name="newFilters"/> for a user so their content is updated to reflect the new filters.
        /// If no filters are passed the no filters have been set for a user the default filters are set for that user.
        /// Persistence of filters depends on the type of IDistrubuted cache registered.
        /// </summary>
        /// <param name="newFilters">The users updated filter settings</param>
        /// <param name="userId">Used for associating a filter with a user so users can have their own dashboard</param>
        /// <returns>A task to track the async update</returns>
        public async Task UpdateFilters(UserFilters filters, uint userId = 0)
        {
            var key = $"{userId}_filters";
            //TODO: add validation for existance of subreddits
            //(I would usuall put TODOs in task tracker to be vetted by the team)
            var filterJson = JsonSerializer.Serialize(filters);
            await cache.SetStringAsync(key, filterJson);
        }

        public async Task GetUserFilters(uint userId = 0)
        {
            var key = $"{userId}_filters";
            var filterJson = await cache.GetStringAsync(key);
            if (filterJson == null)
            {
                // If no filters have been cached for the user we set the default
                filterJson = JsonSerializer.Serialize(settings.Reddit.DefaultFilters);
                await cache.SetStringAsync(key, filterJson);
            }
        }
    }
}
