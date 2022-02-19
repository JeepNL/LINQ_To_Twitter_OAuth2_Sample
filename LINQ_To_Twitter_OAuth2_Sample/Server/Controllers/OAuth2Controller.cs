using LINQ_To_Twitter_OAuth2_Sample.Shared.Models;
using LinqToTwitter;
using LinqToTwitter.Common;
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
	private readonly ILogger<OAuth2Controller> _logger;

	public OAuth2Controller(IConfiguration configuration, ILogger<OAuth2Controller> logger)
	{
		_configuration = configuration;
		_logger = logger;
	}

	[HttpPost]
	public async Task<ActionResult<List<L2TTimeline>>> UserTimeline(L2TBase l2TBase)
	{
		OAuth2Authorizer auth = L2TOAuth2(l2TBase.AccessToken, l2TBase.RefreshToken);
		TwitterContext twitterCtx = new TwitterContext(auth); // #TODO Try/Catch

		TwitterUserQuery userQuery = await (
				from usr in twitterCtx.TwitterUser
				where usr.Type == UserType.IdLookup &&
					usr.Ids == l2TBase.UserId.ToString() &&
					usr.UserFields == UserField.ProfileImageUrl
				select usr
			)
			.SingleOrDefaultAsync();

		TwitterUser user = userQuery.Users.FirstOrDefault();

		TweetQuery tweetQuery = await (
				from tweet in twitterCtx.Tweets
				where tweet.Type == TweetType.TweetsTimeline &&
					tweet.ID == user.ID.ToString() &&
					tweet.MaxResults == 25
				select tweet
			)
			.SingleOrDefaultAsync();

		if (tweetQuery is not null)
		{
			List<L2TTimeline> tweets = (
					from tweet in tweetQuery.Tweets
					select new L2TTimeline
					{
						ImageUrl = user.ProfileImageUrl,
						ScreenName = user.Name,
						Text = tweet.Text
					}
				)
				.ToList();

			return Ok(tweets);
		}
		else
			return NotFound();
	}

	[HttpPost]
	public async Task<ActionResult<L2TTweet>> PostTweet(L2TTweet l2tTweet)
	{
		if (!string.IsNullOrEmpty(l2tTweet.Text))
		{
			OAuth2Authorizer auth = L2TOAuth2(l2tTweet.AccessToken, l2tTweet.RefreshToken);
			TwitterContext twitterCtx = new TwitterContext(auth); // #TODO Try/Catch
			Tweet tweet = new();

			try
			{
				tweet = await twitterCtx.TweetAsync(l2tTweet.Text);
			}
			catch (Exception e)
			{
				_logger.LogError($"***** PostTweet e.StackTrace: {e.StackTrace}");
				l2tTweet.ErrorMessage = e.Message;
				l2tTweet.TweetId = -1;
				return BadRequest(l2tTweet);
			}

			l2tTweet.TweetId = Convert.ToInt64(tweet.ID);
		}
		return Ok(l2tTweet);
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
		OAuth2Authorizer auth = L2TOAuth2(l2tBase.AccessToken, l2tBase.RefreshToken);
		TwitterContext twitterCtx = new TwitterContext(auth); // #TODO Try/Catch

		TwitterUserQuery response = await (
			from usr in twitterCtx.TwitterUser
			where usr.Type == UserType.Me
			select usr
		).SingleOrDefaultAsync();

		TwitterUser user = response?.Users?.SingleOrDefault();

		l2tBase.UserId = Convert.ToInt64(user.ID);

		return Ok(l2tBase);
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

		//_logger.LogInformation("***** Complete() - HttpContext.Session.Keys:");
		//foreach (var key in HttpContext.Session.Keys)
		//{
		//	_logger.LogInformation($"***** key: {key}: {HttpContext.Session.GetString(key)}");
		//}

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
					 $"expire_token_ticks={DateTime.UtcNow.AddMinutes(120).Ticks}&" +
					 $"user_id={user.ID}&" +
					 $"access_denied=false";

		return Redirect(url);
	}

	// Helper(s)
	private static OAuth2Authorizer L2TOAuth2(string accessToken, string refreshToken)
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
