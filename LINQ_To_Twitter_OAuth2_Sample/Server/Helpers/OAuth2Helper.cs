using LinqToTwitter.OAuth;

namespace LINQ_To_Twitter_OAuth2_Sample.Server.Helpers;
public static class OAuth2Helper
{
	public static OAuth2Authorizer Authorizer(string accessToken, string refreshToken)
	{
		return new()
		{
			CredentialStore = new OAuth2CredentialStore()
			{
				AccessToken = accessToken,
				RefreshToken = refreshToken
			}
		};
	}

}
