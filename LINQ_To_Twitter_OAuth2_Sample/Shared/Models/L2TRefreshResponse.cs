namespace LINQ_To_Twitter_OAuth2_Sample.Shared.Models
{
	public class L2TRefreshResponse
	{
		public string token_type { get; set; }
		public int expires_in { get; set; }
		public string access_token { get; set; }
		public string scope { get; set; }
		public string refresh_token { get; set; }
	}
}
