using Reddit.Controllers;
using Reddit.Controllers.Structures;

namespace PaulsRedditFeed
{
    /// <summary>
    /// Model for the raw reddit json payload from the reddit API
    /// </summary>
    public class SerializableSubreddit
    {
        /// <summary>
        /// The banner background color.
        /// </summary>
        public string BannerBackgroundColor { get; set; }

        /// <summary>
        /// The banner background image URL.
        /// </summary>
        public string BannerBackgroundImage { get; set; }

        /// <summary>
        /// The subreddit type (public, restricted, or public)
        /// </summary>
        public string SubredditType { get; set; }

        /// <summary>
        /// The community icon URL.
        /// </summary>
        public string CommunityIcon { get; set; }

        /// <summary>
        /// The header title.
        /// </summary>
        public string HeaderTitle { get; set; }

        /// <summary>
        /// Whether the wiki is enabled for this subreddit.
        /// </summary>
        public bool WikiEnabled { get; set; }

        /// <summary>
        /// Whether you have to be over 18 to view this subreddit.
        /// </summary>
        public bool? Over18 { get; set; }

        /// <summary>
        /// The sidebar text.
        /// </summary>
        public string Sidebar { get; set; }

        /// <summary>
        /// The subreddit name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The subreddit title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Whether to collapse deleted comments.
        /// </summary>
        public bool? CollapseDeletedComments { get; set; }

        /// <summary>
        /// The ID36 of this subreddit.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Whether emojis are enabled.
        /// </summary>
        public bool EmojisEnabled { get; set; }

        /// <summary>
        /// Whether to show media.
        /// </summary>
        public bool? ShowMedia { get; set; }

        /// <summary>
        /// Whether to allow videos.
        /// </summary>
        public bool AllowVideos { get; set; }

        /// <summary>
        /// Whether user flair can be assigned.
        /// </summary>
        public bool CanAssignUserFlair { get; set; }

        /// <summary>
        /// Whether spoilers are enabled.
        /// </summary>
        public bool? SpoilersEnabled { get; set; }

        /// <summary>
        /// The primary color.
        /// </summary>
        public string PrimaryColor { get; set; }

        /// <summary>
        /// The suggested comment sort for this subreddit.
        /// </summary>
        public string SuggestedCommentSort { get; set; }

        /// <summary>
        /// The active user count.
        /// </summary>
        public int? ActiveUserCount { get; set; }

        /// <summary>
        /// Whether link flair can be assigned.
        /// </summary>
        public bool CanAssignLinkFlair { get; set; }

        /// <summary>
        /// Whether to allow video GIFs.
        /// </summary>
        public bool AllowVideoGifs { get; set; }

        /// <summary>
        /// The number of subscribers.
        /// </summary>
        public int? Subscribers { get; set; }

        /// <summary>
        /// The submit text label.
        /// </summary>
        public string SubmitTextLabel { get; set; }

        /// <summary>
        /// The key color.
        /// </summary>
        public string KeyColor { get; set; }

        /// <summary>
        /// The language.
        /// </summary>
        public string Lang { get; set; }

        /// <summary>
        /// The subreddit fullname.
        /// </summary>
        public string Fullname { get; set; }

        /// <summary>
        /// When the subreddit was created.
        /// </summary>
        public DateTime Created { get; set; }

        /// <summary>
        /// The URL.
        /// </summary>
        public string URL { get; set; }

        /// <summary>
        /// The submit link label.
        /// </summary>
        public string SubmitLinkLabel { get; set; }

        /// <summary>
        /// Whether to allow discovery.
        /// </summary>
        public bool? AllowDiscovery { get; set; }

        /// <summary>
        /// The subreddit description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Whether link flair is enabled.
        /// </summary>
        public bool? LinkFlairEnabled { get; set; }

        /// <summary>
        /// Whether to allow images.
        /// </summary>
        public bool? AllowImages { get; set; }

        /// <summary>
        /// How many minutes to hide comment scores.
        /// </summary>
        public int? CommentScoreHideMins { get; set; }

        /// <summary>
        /// Whether to show media previews.
        /// </summary>
        public bool? ShowMediaPreview { get; set; }

        /// <summary>
        /// The submission type.
        /// </summary>
        public string SubmissionType { get; set; }

        /// <summary>
        /// Full subreddit data retrieved from the API.
        /// </summary>
        public Reddit.Things.Subreddit SubredditData { get; set; }

        /// <summary>
        /// Posts belonging to this subreddit.
        /// </summary>
        public SubredditPosts Posts { get; set; }

        /// <summary>
        /// Comments belonging to this subreddit.
        /// </summary>
        public Comments Comments { get; set; }

        /// <summary>
        /// Flairs belonging to this subreddit.
        /// </summary>
        public Flairs Flairs { get; set; }
        public Flairs flairs { get; set; }

        public Wiki wiki { get; set; }

        /// <summary>
        /// Get the submission text for the subreddit.
        /// This text is set by the subreddit moderators and intended to be displayed on the submission form.
        /// </summary>
        public Reddit.Things.SubredditSubmitText SubmitText { get; set; }

        public DateTime? SubmitTextLastUpdated { get; set; }

        /// <summary>
        /// Get the moderators of this subreddit.
        /// </summary>
        public List<Moderator> Moderators { get; set; }
    }
}
