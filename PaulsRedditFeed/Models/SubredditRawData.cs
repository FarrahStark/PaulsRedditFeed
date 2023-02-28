using PaulsRedditFeed;
using Reddit.Controllers;

namespace PaulsRedditFeed
{
    public class SubredditRawData
    {
        public DateTime CollectedTime { get; set; }
        public SerializableSubreddit Payload { get; }
        public Post HottestPost { get; set; }

        public SubredditRawData(DateTime collectedTime, SerializableSubreddit payload, Post hottestPost)
        {
            CollectedTime = collectedTime;
            Payload = payload;
            HottestPost = hottestPost;
        }

        public SubredditRawData(DateTime collectedTime, Subreddit payload, Post hottestPost)
        {
            CollectedTime = collectedTime;
            Payload = payload.ToSerializable();
            HottestPost = hottestPost;
        }
    }
}
