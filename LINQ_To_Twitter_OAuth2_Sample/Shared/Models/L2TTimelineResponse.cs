namespace LINQ_To_Twitter_OAuth2_Sample.Shared.Models;
public class L2TTimelineResponse
{

	public string? TweetId { get; set; }
	public string? ScreenName { get; set; }
	public string? Name { get; set; }
	public string? ProfileImageUrl { get; set; }
	public string? AuthorId { get; set; }
	public DateTime? TweetDate { get; set; }
	public string? Text { get; set; }
	public string? Source { get; set; }
	public L2TTweetEntityUrlDTO[]? Urls { get; set; }
	public L2TTwitterMediaDTO[]? Media { get; set; }

	// #TODO HASHTAGS & MENTIONS
}

// #TODO SEPERATE FILES
public class L2TTweetEntityUrlDTO
{
	public string? Url { get; set; }
	public string? DisplayUrl { get; set; }
	public string? ExpandedUrl { get; set; }
}

public class L2TTwitterMediaDTO
{
	public string? MediaKey { get; set; }
	public L2TTweetMediaType Type { get; set; }
	public string? Url { get; set; }
	public string? PreviewImageUrl { get; set; }
	public string? AltText { get; set; }
	public int Width { get; set; }
	public int Height { get; set; }
	public int DurationMS { get; set; }

}

public enum L2TTweetMediaType
{
	None,
	AnimatedGif,
	Photo,
	Video
}
