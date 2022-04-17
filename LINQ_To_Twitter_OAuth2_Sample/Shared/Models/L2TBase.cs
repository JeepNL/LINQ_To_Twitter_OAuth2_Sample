namespace LINQ_To_Twitter_OAuth2_Sample.Shared.Models;
//public class L2TBase : ICloneable
public class L2TBase
{
	public string UserId { get; set; } = "0";
	public string Name { get; set; } = "";
	public string UserName { get; set; } = "";
	public string ProfileImageUrl { get; set; } = "";
	public string AccessToken { get; set; } = "";
	public string RefreshToken { get; set; } = "";
	public long ExpireTokenTicks { get; set; } = 0;
	public long FollowersCount { get; set; } = 0;
	public long FollowingCount { get; set; } = 0;
	public long TweetCount { get; set; } = 0;
	public long CreatedAtTicks { get; set; } = 0;
	public bool PrivateAccount { get; set; } = false;
	public bool VerifiedAccount { get; set; } = false;
	public string ErrorMessage { get; set; } = "";

	//public object Clone()
	//{
	//	return MemberwiseClone();
	//}

	//public L2TBase(string userId = "0",
	//				string name = "",
	//				string userName = "",
	//				string profileImageUrl = "",
	//				string accessToken = "",
	//				string refreshToken = "",
	//				long expireTokenTicks = 0,
	//				int followerCount = 0,
	//				int followingCount = 0,
	//				int tweetCount = 0,
	//				int createdAtTicks = 0,
	//				bool privateAccount = false,
	//				bool verifiedAccount = false,
	//				string errorMessage = "")
	//{
	//	UserId = userId;
	//	Name = name;
	//	UserName = userName;
	//	ProfileImageUrl = profileImageUrl;
	//	AccessToken = accessToken;
	//	RefreshToken = refreshToken;
	//	ExpireTokenTicks = expireTokenTicks;
	//	FollowerCount = followerCount;
	//	FollowingCount = followingCount;
	//	TweetCount = tweetCount;
	//	CreatedAtTicks = createdAtTicks;
	//	PrivateAccount = privateAccount;
	//	VerifiedAccount = verifiedAccount;
	//	ErrorMessage = errorMessage;
	//}
}
