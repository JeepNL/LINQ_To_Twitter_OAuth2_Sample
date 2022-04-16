using System.ComponentModel.DataAnnotations;

namespace LINQ_To_Twitter_OAuth2_Sample.Shared.Models;
public class L2TTweet : L2TBase
{
	[Required]
	[StringLength(maximumLength: 280, MinimumLength = 1, ErrorMessage = "Text must be between 1 & 280 characters")]
	public string? Text { get; set; }
	public string? TweetId { get; set; }
	public string? AuthorId { get; set; }
	public string? ConversationId { get; set; }
	public List<ImageB64>? ImagesB64 { get; set; }
}

public class ImageB64
{
	public string? FileName { get; set; }
	public string? Data { get; set; }
	public string? Format { get; set; }
}
