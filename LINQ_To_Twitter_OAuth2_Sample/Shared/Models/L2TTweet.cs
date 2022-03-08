using System.ComponentModel.DataAnnotations;

namespace LINQ_To_Twitter_OAuth2_Sample.Shared.Models;
public class L2TTweet : L2TBase
{
	[Required]
	[StringLength(maximumLength: 280, MinimumLength = 2, ErrorMessage = "Text must be between 2 & 280 characters")]
	public string Text { get; set; }
	public long TweetId { get; set; }
	public string ConversationId { get; set; }
	public IEnumerable<string> ImagesB64 { get; set; }
	public L2TTweet(string text = "",
					long tweetId = 0,
					string conversationId = "",
					IEnumerable<string> imagesB64 = null)
	{
		Text = text;
		TweetId = tweetId;
		ConversationId = conversationId;
		ImagesB64 = imagesB64;
	}
}
