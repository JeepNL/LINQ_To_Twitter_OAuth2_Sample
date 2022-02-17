namespace LINQ_To_Twitter_OAuth2_Sample.Shared.Models
{
	public class L2TBase
	{
		public string UserId { get; set; }
		public string AccessToken { get; set; }
		public string RefreshToken { get; set; }
		public long ExpireTokenTicks { get; set; }
		public L2TBase(string userId = "",
						string accessToken = "",
						string refreshToken = "",
						long expireTokenTicks = 0)
		{
			UserId = userId;
			AccessToken = accessToken;
			RefreshToken = refreshToken;
			ExpireTokenTicks = expireTokenTicks;
		}
	}
}
