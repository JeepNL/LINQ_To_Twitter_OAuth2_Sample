namespace LINQ_To_Twitter_OAuth2_Sample.Shared.Models;

//public class L2TTimelineResponse
//{
//	public string? TweetId { get; set; }
//	public string? ScreenName { get; set; }
//	public string? Name { get; set; }
//	public string? ProfileImageUrl { get; set; }
//	public string? AuthorId { get; set; }
//	public DateTime? TweetDate { get; set; }
//	public string? Text { get; set; }
//	public string? Source { get; set; }
//	public L2TEntityMentionDTO[]? Mentions { get; set; }
//	public L2TIncudedTweetDTO[]? ReferencedTweets { get; set; }
//	public L2TEntityUrlDTO[]? Urls { get; set; }
//	public L2TMediaDTO[]? Media { get; set; }
//	public L2TUserDTO[]? Users { get; set; }

//	// #TODO HASHTAGS
//}

public class TimelineResponse
{
    public TweetDTO[]? Tweets { get; set; }
    public IncludedTweetDTO[]? IncludedTweets { get; set; }
    public MediaDTO[]? IncludedMedia { get; set; }
    public UserDTO[]? IncludedUsers { get; set; }
}

public class TweetDTO
{
    public bool IsRetweet { get; set; } = false;
    public string? TweetId { get; set; }
    public string? ScreenName { get; set; }
    public string? Name { get; set; }
    public string? ProfileImageUrl { get; set; }
    public string? AuthorId { get; set; }
    public DateTime? CreatedAt { get; set; }
    public string? Text { get; set; }
    public string? Source { get; set; }
    public string? Lang { get; set; }
    public ReplySettingsDTO ReplySettings { get; set; }
    public string? ConversationId { get; set; }
    public EntityUrlDTO[]? Urls { get; set; }
    public EntityHashtagDTO[]? Hashtags { get; set; }
    public EntityMentionDTO[]? Mentions { get; set; }
    public AttachmentMediaKeyDTO[]? MediaKeys { get; set; }
    public ReferencedTweetDTO[]? ReferencedTweets { get; set; }
}

// #TODO SEPERATE FILES

public class ReferencedTweetDTO
{
    public string? Id { get; set; }
    public string? Type { get; set; }
}

public class IncludedTweetDTO // #TODO NEEDS A LOT OF EXTRA WORK
{
    public string? Id { get; set; }
    public string? Text { get; set; }
    public string? AuthorId { get; set; }
    public EntityUrlDTO[]? Urls { get; set; }

    //public string? Type { get; set; } // #TODO - is not standard, not in includes.tweets (= referencedTweets) but in original tweet.referenced_tweets (type) itself
    //public EntityHashTagDTO[]? HashTags { get; set; }
    //public EntityMentionDTO[]? Mentions { get; set; }

    // #TODO IMAGES?
}

public class EntityUrlDTO
{
    public string? Url { get; set; }
    public string? DisplayUrl { get; set; }
    public string? ExpandedUrl { get; set; }
}

public class EntityMentionDTO
{
    public string? UserName { get; set; } // not in LinqToTwitter EntityMention, bug? #TODO
    public string? Id { get; set; }
}

public class AttachmentMediaKeyDTO
{
    public string? Key { get; set; }
}

public class EntityHashtagDTO
{
    public string? Tag { get; set; }
}

public class UserDTO // in "Includes"
{
    public string? UserName { get; set; }
    public string? Name { get; set; }
    public string? Id { get; set; }
    public string? ProfileImageUrl { get; set; }
}

public class MediaDTO
{
    public string? MediaKey { get; set; }
    public MediaTypeDTO Type { get; set; }
    public string? Url { get; set; }
    public string? PreviewImageUrl { get; set; }
    public string? AltText { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public int DurationMS { get; set; }

}

public enum MediaTypeDTO
{
    None,
    AnimatedGif,
    Photo,
    Video
}

public enum ReplySettingsDTO
{
    None,
    Everyone,
    MentionedUsers,
    Following
}

