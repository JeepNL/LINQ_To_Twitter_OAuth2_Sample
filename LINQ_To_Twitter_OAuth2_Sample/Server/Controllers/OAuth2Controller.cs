using LINQ_To_Twitter_OAuth2_Sample.Shared.Models;
using LinqToTwitter;
using LinqToTwitter.OAuth;
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

		MvcOAuth2Authorizer auth = new()
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
					"offline.access",
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

		string stateId = Guid.NewGuid().ToString().Replace("-","");
		return await auth.BeginAuthorizeAsync(stateId);
	}

	public async Task<IActionResult> Complete()
	{
		OAuth2Authorizer auth = new()
		{
			CredentialStore = new OAuth2SessionCredentialStore(HttpContext.Session)
		};

		Request.Query.TryGetValue("error", out StringValues error);
		Request.Query.TryGetValue("code", out StringValues code);
		Request.Query.TryGetValue("state", out StringValues state);

		if (error == "access_denied") // access cancelled at Twitter
			return Redirect("/l2tcallback?is_authenticated=false");

		await auth.CompleteAuthorizeAsync(code, state);
		IOAuth2CredentialStore credentials = auth.CredentialStore as IOAuth2CredentialStore;

		Console.WriteLine("\n");
		foreach (var key in HttpContext.Session.Keys)
		{
			Console.WriteLine($"***** key: {key}: {HttpContext.Session.GetString(key)}");
		}

		Console.WriteLine($"\n***** credentials.ClientID: {credentials.ClientID}");
		Console.WriteLine($"***** credentials.State: {credentials.State}");
		Console.WriteLine($"***** credentials.ScreenName: {credentials.ScreenName}");
		Console.WriteLine($"***** credentials.UserID: {credentials.UserID}\n\n");

		string url = $"/l2tcallback?access_token={credentials?.AccessToken}&refresh_token={credentials?.RefreshToken}&is_authenticated=true";
		return Redirect(url);
	}

	// #TODO
	//[HttpGet]
	//public async Task<ActionResult<L2TUser>> UserInfo(L2TBase userTokens)
	//{
	//	await Task.Delay(1000);
	//	return Ok();
	//}

	[HttpPost]
	public async Task<ActionResult<L2TTweet>> PostTweet(L2TTweet l2tTweet)
	{
		if (!string.IsNullOrEmpty(l2tTweet.Text))
		{
			OAuth2Authorizer auth = new()
			{
				CredentialStore = new OAuth2CredentialStore
				{
					AccessToken = l2tTweet.AccessToken,
					RefreshToken = l2tTweet.RefreshToken
				}
			};

			TwitterContext twitterCtx = new TwitterContext(auth);
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
