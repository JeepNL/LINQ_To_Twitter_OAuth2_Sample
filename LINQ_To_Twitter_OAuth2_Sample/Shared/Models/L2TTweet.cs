using System.ComponentModel.DataAnnotations;

namespace LINQ_To_Twitter_OAuth2_Sample.Shared.Models
{
    public class L2TTweet : L2TBase
    {
        [Required]
        [StringLength(maximumLength: 280, MinimumLength = 2, ErrorMessage = "Text must be between 2 & 280 characters")]
        public string Text { get; set; } = string.Empty;
        public string TweetId { get; set; } = string.Empty;
        public string ConversationId { get; set; } = string.Empty;
        public string Error { get; set; } = string.Empty;
    }
}
