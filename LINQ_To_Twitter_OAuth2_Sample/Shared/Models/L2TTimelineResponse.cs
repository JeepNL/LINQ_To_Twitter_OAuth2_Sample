namespace LINQ_To_Twitter_OAuth2_Sample.Shared.Models;
public class L2TTimelineResponse
{
	public long TweetId { get; set; }
	public string ScreenName { get; set; }
	public string Name { get; set; }
	public string ProfileImageUrl { get; set; }
	public DateTime? TweetDate { get; set; }
	public string Text { get; set; }
	public string Source { get; set; }
	public List<L2TUrl> Urls { get; set; }
	public List<L2TMedia> Media { get; set; }

	// #TODO HASHTAGS & MENTIONS
}

// #TODO SEPERATE FILES
public class L2TUrl
{
	public string TwitterUrl { get; set; }
	public string DisplayUrl { get; set; }
	public string FullUrl { get; set; }
}

public class L2TMedia
{
	public string Key { get; set; }
	public string Type { get; set; }
	public string Url { get; set; }
	public string PreviewImageUrl { get; set; }
	public string AltText { get; set; }
	public int Width { get; set; }
	public int Height { get; set; }
	public int DurationMS { get; set; }

}
