﻿using LINQ_To_Twitter_OAuth2_Sample.Shared.Models;
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

		var auth = new MvcOAuth2Authorizer
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
					"follows.write",
					"offline.access",
					"space.read",
					"mute.read",
					"mute.write",
					"like.read",
					"like.write",
					"block.read",
					"block.write"
				},
				RedirectUri = twitterCallbackUrl,
			}
		};

		return await auth.BeginAuthorizeAsync("MyState");
	}

	public async Task<IActionResult> Complete()
	{
		var auth = new OAuth2Authorizer
		{
			CredentialStore = new OAuth2SessionCredentialStore(HttpContext.Session)
		};

		Request.Query.TryGetValue("error", out StringValues error);
		Request.Query.TryGetValue("code", out StringValues code);
		Request.Query.TryGetValue("state", out StringValues state);

		if (error == "access_denied") // access cancelled at Twitter
			return Redirect("/l2tcallback?is_authenticated=false");

		await auth.CompleteAuthorizeAsync(code, state);
		IOAuth2CredentialStore? credentials = auth.CredentialStore as IOAuth2CredentialStore;

		TwitterContext? twitterCtx = new TwitterContext(auth);

		Console.WriteLine($"\n\n*****  credentials.ClientID: {twitterCtx.TwitterUser}");
		Console.WriteLine($"*****  credentials?.State: {credentials?.State}");
		Console.WriteLine($"*****  credentials?.ScreenName: {credentials?.ScreenName}");
		Console.WriteLine($"*****  credentials?.UserID: {credentials?.UserID}\n\n");

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
	public async Task<ActionResult<L2TTweet>> PostTweet(L2TTweet postTweet)
	{
        if (!string.IsNullOrEmpty(postTweet.Text))
        {
			OAuth2Authorizer? auth = new()
			{
				CredentialStore = new OAuth2CredentialStore
				{
					AccessToken = postTweet.AccessToken,
					RefreshToken = postTweet.RefreshToken
				}
			};

			TwitterContext? twitterCtx = new TwitterContext(auth);
			Tweet? tweet = await twitterCtx.TweetAsync(postTweet.Text); // #TODO (Try/Catch)
			postTweet.TweetId = tweet?.ID ?? "-1"; // "-1" (Auth Failed)
		}
		return Ok(postTweet);
	}
}
