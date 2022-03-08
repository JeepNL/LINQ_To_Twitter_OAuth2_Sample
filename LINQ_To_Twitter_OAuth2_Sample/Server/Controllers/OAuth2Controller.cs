using LINQ_To_Twitter_OAuth2_Sample.Shared.Models;
using LinqToTwitter;
using LinqToTwitter.Common;
using LinqToTwitter.OAuth;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using System.Web;

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
	public async Task<ActionResult<List<L2TTimelineResponse>>> UserTimeline(L2TTimelineRequest l2tTimelineRequest)
	{
		OAuth2Authorizer auth = L2TOAuth2(l2tTimelineRequest.AccessToken, l2tTimelineRequest.RefreshToken);
		TwitterContext twitterCtx = new TwitterContext(auth); // #TODO Try/Catch

		TweetQuery tweetQuery = await (
				from tweet in twitterCtx.Tweets
				where tweet.Type == TweetType.TweetsTimeline
					&& tweet.ID == l2tTimelineRequest.ForUserId.ToString() // tweet.ID = Tweet AuthorId, not the id of the tweet.
					&& tweet.UserFields == $"{UserField.AllFields}"
					&& tweet.TweetFields == $"{TweetField.AllFieldsExceptPermissioned}"
					&& tweet.Expansions == $"{ExpansionField.MediaKeys},{ExpansionField.AuthorID}"
					&& tweet.MediaFields == $"{MediaField.AllFieldsExceptPermissioned}"
					//&& tweet.PlaceFields == $"{PlaceField.AllFields}"
					&& tweet.MaxResults == l2tTimelineRequest.MaxResults // default = 10
					&& tweet.SinceID == l2tTimelineRequest.SinceId.ToString() // default = 0
				select tweet
		).SingleOrDefaultAsync();

		if (tweetQuery is not null)
		{
			// TweetType.TweetsTimeline has only 1 author
			// tweetQuery.Includes.Users works because of ExpansionField.AuthorID
			TwitterUser author = tweetQuery.Includes.Users.Where(twitterUser => twitterUser.ID == l2tTimelineRequest.ForUserId.ToString()).FirstOrDefault();

			List<L2TTimelineResponse> tlResponse = (
					from tweet in tweetQuery.Tweets
					select new L2TTimelineResponse
					{
						TweetId = Convert.ToInt64(tweet.ID),
						ScreenName = author.Username,
						Name = author.Name,
						ProfileImageUrl = author.ProfileImageUrl,
						Text = tweet.Text.Replace("\n", "<br />"),
						TweetDate = tweet.CreatedAt,
						Source = tweet.Source,
						Urls = (from entityUrl in tweet.Entities.Urls
								select new L2TUrl
								{
									TwitterUrl = entityUrl.Url,
									DisplayUrl = entityUrl.DisplayUrl,
									FullUrl = HttpUtility.UrlEncode(entityUrl.ExpandedUrl),
								}).ToList(),
						Media = (from mediaKey in tweet.Attachments.MediaKeys
								 select new L2TMedia
								 {
									 Key = mediaKey,
									 Type = tweetQuery.Includes.Media.Where(key => key.MediaKey == mediaKey).FirstOrDefault().Type.ToString(),
									 Url = tweetQuery.Includes.Media.Where(key => key.MediaKey == mediaKey).FirstOrDefault().Url.ToString(),
									 PreviewImageUrl = tweetQuery.Includes.Media.Where(key => key.MediaKey == mediaKey).FirstOrDefault().PreviewImageUrl?.ToString(),
									 AltText = tweetQuery.Includes.Media.Where(key => key.MediaKey == mediaKey).FirstOrDefault().AltText?.ToString(),
									 Width = tweetQuery.Includes.Media.Where(key => key.MediaKey == mediaKey).FirstOrDefault().Width,
									 Height = tweetQuery.Includes.Media.Where(key => key.MediaKey == mediaKey).FirstOrDefault().Height,
									 DurationMS = tweetQuery.Includes.Media.Where(key => key.MediaKey == mediaKey).FirstOrDefault().DurationMS,
								 }).ToList(),
						// #TODO MENTIONS * HASHTAGS
					}
			).ToList();

			return Ok(tlResponse);
		}
		else
			return NotFound(); // #TODO, isn't 'NotFound', but 'No Results' ...
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

	// Utilities
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

/// TEST CODE

//_logger.LogInformation("***** Complete() - HttpContext.Session.Keys:");
//foreach (var key in HttpContext.Session.Keys)
//{
//	_logger.LogInformation($"***** key: {key}: {HttpContext.Session.GetString(key)}");
//}

//TwitterUserQuery userQuery = await (
//		from usr in twitterCtx.TwitterUser
//		where usr.Type == UserType.IdLookup
//			&& usr.Ids == l2tTimelineRequest.ForUserId.ToString()
//			&& usr.UserFields == $"{UserField.ProfileImageUrl}"
//		select usr
//	)
//	.SingleOrDefaultAsync();
//TwitterUser user = userQuery.Users.FirstOrDefault();

//List<L2TTimelineResponse> tweets2 = await (
//		from tweet in twitterCtx.Tweets
//		where tweet.Type == TweetType.TweetsTimeline
//			&& tweet.ID == l2tTimelineRequest.ForUserId.ToString()
//			&& tweet.UserFields == $"{UserField.AllFields}"
//			&& tweet.TweetFields == $"{TweetField.AllFieldsExceptPermissioned}"
//			&& tweet.Expansions == $"{ExpansionField.MediaKeys},{ExpansionField.AuthorID}"
//			&& tweet.MediaFields == $"{MediaField.AllFieldsExceptPermissioned}"
//			&& tweet.PlaceFields == $"{PlaceField.AllFields}"
//			&& tweet.MaxResults == l2tTimelineRequest.MaxResults
//			&& tweet.SinceID == l2tTimelineRequest.SinceId.ToString()
//		select new L2TTimelineResponse
//		{
//			ProfileImageUrl = tweet.User.ProfileImageUrl,
//			ScreenName = tweet.User.ScreenNameResponse,
//			Text = tweet.FullText ?? tweet.Text
//		}).ToListAsync();
//return Ok(tweets2);

//foreach (Tweet twt in tweetQuery.Tweets)
//{
//	Console.WriteLine($"\n***** START FULLTWEET: {twt.Text}");
//	if (twt.Entities?.Urls is not null)
//	{
//		Console.WriteLine($"***** START URLS *****");
//		foreach (TweetEntityUrl url in twt.Entities.Urls)
//			Console.WriteLine($"     ***** twt url.Url: {url.Url} - url.DisplayUrl: {url.DisplayUrl} - url.ExpandedUrl: {url.ExpandedUrl}");
//		Console.WriteLine($"***** END URLS *****\n");
//	}
//	if (twt.Entities?.Hashtags is not null)
//	{
//		Console.WriteLine($"***** START HASHTAGS *****");
//		foreach (TweetEntityHashtag hashtag in twt.Entities.Hashtags)
//			Console.WriteLine($"***** twt hashtag.Tag: {hashtag.Tag}");
//		Console.WriteLine($"***** END HASHTAGS *****\n");
//	}

//	if (twt.Attachments?.MediaKeys is not null)
//	{
//		Console.WriteLine($"***** START MEDIAKEYS *****");
//		foreach (string mediaKey in twt.Attachments.MediaKeys)
//		{
//			TwitterMedia twitterMedia = tweetQuery.Includes.Media.Where(key => key.MediaKey == mediaKey).FirstOrDefault();
//			Console.WriteLine($"***** twt.ID: {twt.ID} - mf.Type: {twitterMedia.Type} - mf.MediaKey: {twitterMedia.MediaKey} - mf.Url: {twitterMedia.Url}");
//		}
//		Console.WriteLine($"***** END MEDIAKEYS *****\n");
//	}
//	Console.WriteLine($"***** END FULLTWEET");
//}