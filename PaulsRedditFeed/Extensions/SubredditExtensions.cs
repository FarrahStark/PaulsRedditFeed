using Reddit.Controllers;
using StackExchange.Redis;
using System.Reflection;

namespace PaulsRedditFeed
{
    public static class SubredditExtensions
    {
        private static PropertyInfo[] serializableProperties = GetProperties<SerializableSubreddit>();
        private static Dictionary<string, PropertyInfo> redditProperties = GetProperties<Subreddit>().ToDictionary(p => p.Name, p => p);

        /// <summary>
        /// Converts the <paramref name="subreddit"/> to a serializable model
        /// </summary>
        /// <param name="subreddit">The Reddit.NET subreddit controller to convert</param>
        /// <returns>a serializable subreddit model</returns>
        public static SerializableSubreddit ToSerializable(this Subreddit subreddit)
        {
            var serializableSubreddit = new SerializableSubreddit();
            try
            {
                foreach (var prop in serializableProperties)
                {
                    if (!redditProperties.ContainsKey(prop.Name) ||
                        !redditProperties.ContainsKey(prop.Name) ||
                        redditProperties[prop.Name].PropertyType != prop.PropertyType)
                    {
                        // ignore properties not on the serializable model
                        continue;
                    }

                    var redditProp = redditProperties[prop.Name];
                    prop.SetValue(serializableSubreddit, redditProp.GetValue(subreddit));
                }
            }
            catch (Exception ex)
            {

            }
            return serializableSubreddit;
        }

        private static PropertyInfo[] GetProperties<T>()
        {
            return typeof(T).GetProperties(
                BindingFlags.Instance |
                BindingFlags.Public) ?? new PropertyInfo[0];
        }
    }
}
