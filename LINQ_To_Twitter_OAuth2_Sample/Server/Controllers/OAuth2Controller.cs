using LINQ_To_Twitter_OAuth2_Sample.Shared.Models;
using LinqToTwitter;
using LinqToTwitter.OAuth;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

namespace LINQ_To_Twitter_OAuth2_Sample.Server.Controllers;

[Route("[controller]/[action]")]
[ApiController]

public class OAuth2Controller : ControllerBase
{
	private readonly IConfiguration _configuration;

	public OAuth2Controller(IConfiguration configuration)
	{
		_configuration = configuration;
	}

	public async Task<IActionResult> Begin()
	{
		string twitterCallbackUrl = Request.GetDisplayUrl().Replace("Begin", "Complete");

		MvcOAuth2Authorizer auth = new MvcOAuth2Authorizer()
		{
			CredentialStore = new OAuth2SessionCredentialStore(HttpContext.Session)
			{
				ClientID = _configuration["TwitterClientID"],
				ClientSecret = _configuration["TwitterClientSecret"],
				Scopes = new List<string>
				{
					"tweet.read",
					"tweet.write",
					"tweet.moderate.write",
					"users.read",
					"follows.read",
					//"follows.write",
					"offline.access", // needed for the 'Refresh Token' - Important
					//"space.read",
					"mute.read",
					//"mute.write",
					"like.read",
					//"like.write",
					"block.read",
					//"block.write"
				},
				RedirectUri = twitterCallbackUrl,
			}
		};

		string stateId = Guid.NewGuid().ToString().Replace("-", "");
		return await auth.BeginAuthorizeAsync(stateId);
	}

	public async Task<IActionResult> Complete()
	{
		Request.Query.TryGetValue("error", out StringValues error);
		Request.Query.TryGetValue("code", out StringValues code);
		Request.Query.TryGetValue("state", out StringValues state);

		if (error == "access_denied") // access cancelled at Twitter
			return Redirect("/l2tcallback?access_denied=true");

		OAuth2Authorizer auth = new()
		{
			CredentialStore = new OAuth2SessionCredentialStore(HttpContext.Session)
		};

		//Console.WriteLine("\n\n***** Complete:");
		//foreach (var key in HttpContext.Session.Keys)
		//{
		//	Console.WriteLine($"***** key: {key}: {HttpContext.Session.GetString(key)}");
		//}
		//Console.WriteLine("\n");

		await auth.CompleteAuthorizeAsync(code, state);

		IOAuth2CredentialStore credentials = auth.CredentialStore as IOAuth2CredentialStore;

		// Get 'Me' User(Id)
		TwitterContext twitterCtx = new TwitterContext(auth);
		TwitterUserQuery response = await (
			from usr in twitterCtx.TwitterUser
			where usr.Type == UserType.Me
			select usr
		).SingleOrDefaultAsync();
		TwitterUser user = response?.Users?.SingleOrDefault();

		string url = $"/l2tcallback?access_token={credentials.AccessToken}&" +
					 $"refresh_token={credentials.RefreshToken}&" +
					 $"expire_token_ticks={DateTime.UtcNow.AddMinutes(120).Ticks}&user_id={user.ID}";

		return Redirect(url);
	}

	[HttpPost]
	public async Task<string> RefreshToken(L2TBase l2tBase)
	{
		OAuth2Authorizer auth = new()
		{
			CredentialStore = new OAuth2CredentialStore()
			{
				ClientID = _configuration["TwitterClientID"],
				ClientSecret = _configuration["TwitterClientSecret"],
				RefreshToken = l2tBase.RefreshToken
			}
		};

		return await auth.RefreshTokenAsync();
	}

	[HttpPost]
	public async Task<string> RevokeToken(L2TBase l2tBase)
	{
		OAuth2Authorizer auth = new()
		{
			CredentialStore = new OAuth2CredentialStore()
			{
				ClientID = _configuration["TwitterClientID"],
				ClientSecret = _configuration["TwitterClientSecret"],
				AccessToken = l2tBase.AccessToken,
			}
		};

		string result = await auth.RevokeTokenAsync(); // revokes app authorization

		// delete session cookie
		HttpContext.Session.Clear();

		return result;
	}

	[HttpPost]
	public async Task<ActionResult<L2TBase>> UserInfo(L2TBase l2tBase)
	{
		OAuth2Authorizer auth = new()
		{
			CredentialStore = new OAuth2CredentialStore()
			{
				AccessToken = l2tBase.AccessToken,
				RefreshToken = l2tBase.RefreshToken
			}
		};

		TwitterContext twitterCtx = new TwitterContext(auth); // #TODO Try/Catch
		TwitterUserQuery response = await (
			from usr in twitterCtx.TwitterUser
			where usr.Type == UserType.Me
			select usr
		).SingleOrDefaultAsync();

		TwitterUser user = response?.Users?.SingleOrDefault();

		l2tBase.UserId = user.ID;

		return Ok(l2tBase);
	}

	[HttpPost]
	public async Task<ActionResult<L2TTweet>> PostTweet(L2TTweet l2tTweet)
	{
		if (!string.IsNullOrEmpty(l2tTweet.Text))
		{
			OAuth2Authorizer auth = new()
			{
				CredentialStore = new OAuth2CredentialStore()
				{
					AccessToken = l2tTweet.AccessToken,
					RefreshToken = l2tTweet.RefreshToken,
				}
			};

			TwitterContext twitterCtx = new TwitterContext(auth); // #TODO Try/Catch
			Tweet tweet = new();

			try
			{
				tweet = await twitterCtx.TweetAsync(l2tTweet.Text);
			}
			catch (Exception e)
			{
				l2tTweet.Error = e.Message;
				Console.WriteLine($"\n\n***** e.StackTrace: {e.StackTrace}\n\n");
			}

			l2tTweet.TweetId = tweet.ID ?? "-1";
		}
		return Ok(l2tTweet);
	}
}
