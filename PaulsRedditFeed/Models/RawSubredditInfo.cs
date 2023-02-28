namespace PaulsRedditFeed;

public class SubredditDataMessage
{
    public RawSubredditInfo Subreddit { get; set; }
    public HotPostRawData HotPosts { get; set; }
}

public class RawSubredditInfo
{
    public string kind { get; set; }
    public SubredditInfoData data { get; set; }
}

public class SubredditInfoData
{
    public object user_flair_background_color { get; set; }
    public object submit_text_html { get; set; }
    public bool restrict_posting { get; set; }
    public object user_is_banned { get; set; }
    public bool free_form_reports { get; set; }
    public bool wiki_enabled { get; set; }
    public object user_is_muted { get; set; }
    public object user_can_flair_in_sr { get; set; }
    public string display_name { get; set; }
    public string header_img { get; set; }
    public string title { get; set; }
    public bool allow_galleries { get; set; }
    public object icon_size { get; set; }
    public string primary_color { get; set; }
    public int active_user_count { get; set; }
    public string icon_img { get; set; }
    public string display_name_prefixed { get; set; }
    public int accounts_active { get; set; }
    public bool public_traffic { get; set; }
    public int subscribers { get; set; }
    public object[] user_flair_richtext { get; set; }
    public int videostream_links_count { get; set; }
    public string name { get; set; }
    public bool quarantine { get; set; }
    public bool hide_ads { get; set; }
    public string prediction_leaderboard_entry_type { get; set; }
    public bool emojis_enabled { get; set; }
    public string advertiser_category { get; set; }
    public string public_description { get; set; }
    public int comment_score_hide_mins { get; set; }
    public bool allow_predictions { get; set; }
    public object user_has_favorited { get; set; }
    public object user_flair_template_id { get; set; }
    public string community_icon { get; set; }
    public string banner_background_image { get; set; }
    public bool original_content_tag_enabled { get; set; }
    public bool community_reviewed { get; set; }
    public string submit_text { get; set; }
    public string description_html { get; set; }
    public bool spoilers_enabled { get; set; }
    public bool allow_talks { get; set; }
    public int[] header_size { get; set; }
    public string user_flair_position { get; set; }
    public bool all_original_content { get; set; }
    public bool has_menu_widget { get; set; }
    public object is_enrolled_in_new_modmail { get; set; }
    public string key_color { get; set; }
    public bool can_assign_user_flair { get; set; }
    public float created { get; set; }
    public int wls { get; set; }
    public bool show_media_preview { get; set; }
    public string submission_type { get; set; }
    public object user_is_subscriber { get; set; }
    public object[] allowed_media_in_comments { get; set; }
    public bool allow_videogifs { get; set; }
    public bool should_archive_posts { get; set; }
    public string user_flair_type { get; set; }
    public bool allow_polls { get; set; }
    public bool collapse_deleted_comments { get; set; }
    public object emojis_custom_size { get; set; }
    public string public_description_html { get; set; }
    public bool allow_videos { get; set; }
    public bool is_crosspostable_subreddit { get; set; }
    public object notification_level { get; set; }
    public bool should_show_media_in_comments_setting { get; set; }
    public bool can_assign_link_flair { get; set; }
    public bool accounts_active_is_fuzzed { get; set; }
    public bool allow_prediction_contributors { get; set; }
    public string submit_text_label { get; set; }
    public string link_flair_position { get; set; }
    public object user_sr_flair_enabled { get; set; }
    public bool user_flair_enabled_in_sr { get; set; }
    public bool allow_chat_post_creation { get; set; }
    public bool allow_discovery { get; set; }
    public bool accept_followers { get; set; }
    public bool user_sr_theme_enabled { get; set; }
    public bool link_flair_enabled { get; set; }
    public bool disable_contributor_requests { get; set; }
    public string subreddit_type { get; set; }
    public object suggested_comment_sort { get; set; }
    public string banner_img { get; set; }
    public object user_flair_text { get; set; }
    public string banner_background_color { get; set; }
    public bool show_media { get; set; }
    public string id { get; set; }
    public object user_is_moderator { get; set; }
    public bool over18 { get; set; }
    public string header_title { get; set; }
    public string description { get; set; }
    public bool is_chat_post_feature_enabled { get; set; }
    public string submit_link_label { get; set; }
    public object user_flair_text_color { get; set; }
    public bool restrict_commenting { get; set; }
    public object user_flair_css_class { get; set; }
    public bool allow_images { get; set; }
    public string lang { get; set; }
    public string whitelist_status { get; set; }
    public string url { get; set; }
    public float created_utc { get; set; }
    public object banner_size { get; set; }
    public string mobile_banner_image { get; set; }
    public object user_is_contributor { get; set; }
    public bool allow_predictions_tournament { get; set; }
}
