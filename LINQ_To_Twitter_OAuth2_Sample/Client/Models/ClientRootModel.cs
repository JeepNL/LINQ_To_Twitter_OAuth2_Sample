using System.Text.Json.Serialization;

namespace LINQ_To_Twitter_OAuth2_Sample.Client.Models;

// The servers sends the complete L2T JSON response to the client,
// thats why these records are needed on the client to deserialize the JSON response.
// Work in progress, ie: 'object'

public record ClientAttachments
(
	[property: JsonPropertyName("poll_ids")] IEnumerable<string>? PollIds,
	[property: JsonPropertyName("media_keys")] IEnumerable<string>? MediaKeys
);

public record ClientUrl
(
	[property: JsonPropertyName("description")] string? Description,
	[property: JsonPropertyName("display_url")] string? DisplayUrl,
	[property: JsonPropertyName("end")] int End,
	[property: JsonPropertyName("expanded_url")] string? ExpandedUrl,
	[property: JsonPropertyName("images")] object? Images,
	[property: JsonPropertyName("start")] int Start,
	[property: JsonPropertyName("status")] int? Status,
	[property: JsonPropertyName("title")] string? Title,
	[property: JsonPropertyName("unwound_url")] string? UnwoundUrl,
	[property: JsonPropertyName("url")] string? TwitterUrl
);

public record ClientHashtag
(
	[property: JsonPropertyName("start")] int Start,
	[property: JsonPropertyName("end")] int End,
	[property: JsonPropertyName("tag")] string? Tag
);
public record ClientMention
(
	[property: JsonPropertyName("start")] int Start,
	[property: JsonPropertyName("end")] int End,
	[property: JsonPropertyName("username")] string? Username
);
public record ClientEntities
(
	[property: JsonPropertyName("urls")] IEnumerable<ClientUrl>? Urls,
	[property: JsonPropertyName("hashtags")] IEnumerable<ClientHashtag>? Hashtags,
	[property: JsonPropertyName("mentions")] IEnumerable<ClientMention>? Mentions,
	[property: JsonPropertyName("annotations")] object? Annotations,
	[property: JsonPropertyName("url")] ClientUrl? Url,
	[property: JsonPropertyName("description")] ClientDescription? Description
);
public record ClientPublicMetrics
(
	[property: JsonPropertyName("retweet_count")] int RetweetCount,
	[property: JsonPropertyName("reply_count")] int ReplyCount,
	[property: JsonPropertyName("like_count")] int LikeCount,
	[property: JsonPropertyName("quote_count")] int QuoteCount,
	[property: JsonPropertyName("followers_count")] int FollowersCount,
	[property: JsonPropertyName("following_count")] int FollowingCount,
	[property: JsonPropertyName("tweet_count")] int TweetCount,
	[property: JsonPropertyName("listed_count")] int ListedCount
);
public record ClientReferencedTweet
(
	[property: JsonPropertyName("type")] string? Type,
	[property: JsonPropertyName("id")] string? Id
);
public record ClientData
(
	[property: JsonPropertyName("attachments")] ClientAttachments? Attachments,
	[property: JsonPropertyName("author_id")] string? AuthorId,
	[property: JsonPropertyName("context_annotations")] object? ContextAnnotations,
	[property: JsonPropertyName("conversation_id")] string? ConversationId,
	[property: JsonPropertyName("created_at")] DateTime CreatedAt,
	[property: JsonPropertyName("entities")] ClientEntities? Entities,
	[property: JsonPropertyName("geo")] object? Geo,
	[property: JsonPropertyName("id")] string? Id,
	[property: JsonPropertyName("in_reply_to_user_id")] string? InReplyToUserId,
	[property: JsonPropertyName("lang")] string? Lang,
	[property: JsonPropertyName("non_public_metrics")] object? NonPublicMetrics,
	[property: JsonPropertyName("organic_metrics")] object? OrganicMetrics,
	[property: JsonPropertyName("possibly_sensitive")] bool PossiblySensitive,
	[property: JsonPropertyName("promoted_metrics")] object? PromotedMetrics,
	[property: JsonPropertyName("public_metrics")] ClientPublicMetrics? PublicMetrics,
	[property: JsonPropertyName("referenced_tweets")] IEnumerable<ClientReferencedTweet>? ReferencedTweets,
	[property: JsonPropertyName("reply_settings")] int ReplySettings,
	[property: JsonPropertyName("source")] string? Source,
	[property: JsonPropertyName("text")] string? Text,
	[property: JsonPropertyName("withheld")] object? Withheld
);
public record ClientUrl2
(
	[property: JsonPropertyName("urls")] IEnumerable<ClientUrl>? Urls
);

public record ClientDescription
(
	[property: JsonPropertyName("urls")] object? Urls,
	[property: JsonPropertyName("hashtags")] IEnumerable<ClientHashtag>? Hashtags,
	[property: JsonPropertyName("mentions")] IEnumerable<ClientMention>? Mentions
);
public record ClientUser
(
	[property: JsonPropertyName("created_at")] DateTime CreatedAt,
	[property: JsonPropertyName("description")] string? Description,
	[property: JsonPropertyName("entities")] ClientEntities? Entities,
	[property: JsonPropertyName("id")] string? Id,
	[property: JsonPropertyName("location")] string? Location,
	[property: JsonPropertyName("name")] string? Name,
	[property: JsonPropertyName("pinned_tweet_id")] string? PinnedTweetId,
	[property: JsonPropertyName("profile_image_url")] string? ProfileImageUrl,
	[property: JsonPropertyName("protected")] bool Protected,
	[property: JsonPropertyName("public_metrics")] ClientPublicMetrics? PublicMetrics,
	[property: JsonPropertyName("url")] string? Url,
	[property: JsonPropertyName("username")] string? Username,
	[property: JsonPropertyName("verified")] bool Verified,
	[property: JsonPropertyName("Withheld")] object? Withheld
);
public record ClientDomain
(
	[property: JsonPropertyName("id")] string? Id,
	[property: JsonPropertyName("name")] string? Name,
	[property: JsonPropertyName("description")] string? Description
);
public record ClientEntity
(
	[property: JsonPropertyName("id")] string? Id,
	[property: JsonPropertyName("name")] string? Name,
	[property: JsonPropertyName("description")] string? Description
);
public record ClientContextAnnotation
(
	[property: JsonPropertyName("domain")] ClientDomain? Domain,
	[property: JsonPropertyName("entity")] ClientEntity? Entity
);
public record ClientImage
(
	[property: JsonPropertyName("height")] int Height,
	[property: JsonPropertyName("url")] string? Url,
	[property: JsonPropertyName("width")] int Width
);
public record ClientAnnotation
(
	[property: JsonPropertyName("start")] int Start,
	[property: JsonPropertyName("end")] int End,
	[property: JsonPropertyName("probability")] double Probability,
	[property: JsonPropertyName("type")] string? Type,
	[property: JsonPropertyName("normalized_text")] string? NormalizedText
);
public record ClientTweet
(
	[property: JsonPropertyName("attachments")] ClientAttachments? Attachments,
	[property: JsonPropertyName("author_id")] string? AuthorId,
	[property: JsonPropertyName("context_annotations")] IEnumerable<ClientContextAnnotation>? ContextAnnotations,
	[property: JsonPropertyName("conversation_id")] string? ConversationId,
	[property: JsonPropertyName("created_at")] DateTime CreatedAt,
	[property: JsonPropertyName("entities")] ClientEntities? Entities,
	[property: JsonPropertyName("geo")] object? Geo,
	[property: JsonPropertyName("id")] string? Id,
	[property: JsonPropertyName("in_reply_to_user_id")] string? InReplyToUserId,
	[property: JsonPropertyName("lang")] string? Lang,
	[property: JsonPropertyName("non_public_metrics")] object? NonPublicMetrics,
	[property: JsonPropertyName("organic_metrics")] object? OrganicMetrics,
	[property: JsonPropertyName("possibly_sensitive")] bool PossiblySensitive,
	[property: JsonPropertyName("promoted_metrics")] object? PromotedMetrics,
	[property: JsonPropertyName("public_metrics")] ClientPublicMetrics? PublicMetrics,
	[property: JsonPropertyName("referenced_tweets")] IEnumerable<ClientReferencedTweet>? ReferencedTweets,
	[property: JsonPropertyName("reply_settings")] int ReplySettings,
	[property: JsonPropertyName("source")] string? Source,
	[property: JsonPropertyName("text")] string? Text,
	[property: JsonPropertyName("withheld")] object? Withheld
);
public record ClientMedia
(
	[property: JsonPropertyName("alt_text")] string? AltText,
	[property: JsonPropertyName("duration_ms")] int DurationMs,
	[property: JsonPropertyName("height")] int Height,
	[property: JsonPropertyName("media_key")] string? MediaKey,
	[property: JsonPropertyName("non_public_metrics")] object? NonPublicMetrics,
	[property: JsonPropertyName("organic_metrics")] object? OrganicMetrics,
	[property: JsonPropertyName("preview_image_url")] string? PreviewImageUrl,
	[property: JsonPropertyName("promoted_metrics")] object? PromotedMetrics,
	[property: JsonPropertyName("public_metrics")] ClientPublicMetrics? PublicMetrics,
	[property: JsonPropertyName("type")] string? Type,
	[property: JsonPropertyName("url")] string? Url,
	[property: JsonPropertyName("width")] int Width
);
public record ClientIncludes
(
	[property: JsonPropertyName("users")] IEnumerable<ClientUser>? Users,
	[property: JsonPropertyName("tweets")] IEnumerable<ClientTweet>? Tweets,
	[property: JsonPropertyName("places")] object? Places,
	[property: JsonPropertyName("media")] IEnumerable<ClientMedia>? Medias
);
public record ClientMeta
(
	[property: JsonPropertyName("result_count")] int ResultCount,
	[property: JsonPropertyName("previous_token")] object? PreviousToken,
	[property: JsonPropertyName("next_token")] object? NextToken
);
public record ClientRootResponse
(
	[property: JsonPropertyName("Type")] int Type,
	[property: JsonPropertyName("EndTime")] DateTime EndTime,
	[property: JsonPropertyName("Exclude")] object? Exclude,
	[property: JsonPropertyName("Expansions")] string? Expansions,
	[property: JsonPropertyName("Ids")] string? Ids,
	[property: JsonPropertyName("ID")] string? ID,
	[property: JsonPropertyName("ListID")] object? ListID,
	[property: JsonPropertyName("MaxResults")] int MaxResults,
	[property: JsonPropertyName("MediaFields")] string? MediaFields,
	[property: JsonPropertyName("PaginationToken")] object? PaginationToken,
	[property: JsonPropertyName("PlaceFields")] string? PlaceFields,
	[property: JsonPropertyName("PollFields")] string? PollFields,
	[property: JsonPropertyName("SinceID")] string? SinceID,
	[property: JsonPropertyName("StartTime")] DateTime StartTime,
	[property: JsonPropertyName("SpaceID")] object? SpaceID,
	[property: JsonPropertyName("TweetFields")] string? TweetFields,
	[property: JsonPropertyName("UntilID")] object? UntilID,
	[property: JsonPropertyName("UserFields")] string? UserFields,
	[property: JsonPropertyName("data")] IEnumerable<ClientData>? Data,
	[property: JsonPropertyName("errors")] object? Errors,
	[property: JsonPropertyName("HasErrors")] bool HasErrors,
	[property: JsonPropertyName("includes")] ClientIncludes? Includes,
	[property: JsonPropertyName("meta")] ClientMeta? Meta
);
