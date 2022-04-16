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

	public async Task<IActionResult> Begin()
	{
		string twitterCallbackUrl = Request.GetDisplayUrl().Replace("Begin", "Complete");

		MvcOAuth2Authorizer auth = new()
		{
			CredentialStore = new OAuth2SessionCredentialStore(HttpContext.Session)
			{
				ClientID = _configuration["TwitterClientID"],
				ClientSecret = _configuration["TwitterClientSecret"],
				// Scopes see: https://developer.twitter.com/en/docs/authentication/oauth-2-0/authorization-code
				Scopes = new List<string>
				{
					"tweet.read",
					"tweet.write",
					"tweet.moderate.write", // Hide and unhide replies to your Tweets.
					"users.read",
					"follows.read",
					"follows.write",
					"offline.access", // needed for the 'Refresh Token' - Important
					"space.read",
					"mute.read",
					"mute.write",
					"like.read", // Favorites
					"like.write",
					"block.read",
					"block.write",
					//"bookmark.read", // not yet implemented in L2T?
					//"bookmark.write", // not yet implemented in L2T?
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

		if (error.ToString() == "access_denied") // access cancelled at Twitter
			return Redirect("/l2tcallback?AccessDenied=true");

		OAuth2Authorizer? auth = new()
		{
			CredentialStore = new OAuth2SessionCredentialStore(HttpContext.Session)
		};

		await auth.CompleteAuthorizeAsync(code.ToString(), state.ToString());

		IOAuth2CredentialStore? credentials = auth.CredentialStore as IOAuth2CredentialStore;

		// Get 'Me' User(Id)
		TwitterContext twitterCtx = new(auth);
		TwitterUserQuery? response = await (
			from usr in twitterCtx.TwitterUser
			where usr.Type == UserType.Me
				&& usr.Expansions == $"{ExpansionField.AllUserFields}"
				&& usr.UserFields == $"{UserField.AllFields}"
			select usr
		).SingleOrDefaultAsync();

		TwitterUser? user = response?.Users?.SingleOrDefault();

		string url = $"/l2tcallback?AccessToken={credentials?.AccessToken}&" +
					 $"RefreshToken={credentials?.RefreshToken}&" +
					 $"ExpireTokenTicks={DateTime.UtcNow.AddMinutes(120).Ticks}&" +
					 $"UserId={user?.ID}&" +
					 $"Name={HttpUtility.UrlEncode(user?.Name)}&" +
					 $"UserName={HttpUtility.UrlEncode(user?.Username)}&" +
					 $"ProfileImageUrl={HttpUtility.UrlEncode(user?.ProfileImageUrl)}&" +
					 $"FollowersCount={user?.PublicMetrics?.FollowersCount}&" +
					 $"FollowingCount={user?.PublicMetrics?.FollowingCount}&" +
					 $"TweetCount={user?.PublicMetrics?.TweetCount}&" +
					 $"CreatedAtTicks={user?.CreatedAt.Ticks}&" +
					 $"PrivateAccount={user?.Protected}&" +
					 $"VerifiedAccount={user?.Verified}&" +
					 $"AccessDenied=false";

		return Redirect(url);
	}

	[HttpPost]

	public async Task<ActionResult<string>> RefreshToken(L2TBase l2tBase)
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

		string? authString = await auth.RefreshTokenAsync();

		Console.WriteLine($"\n***** authString: {authString}\n");

		//// #TODO User may have changed his UserName, Handle (Name) and/or ProfileImage, new call to 👇
		//TwitterContext twitterCtx = new(auth);
		//TwitterUserQuery? response = await (
		//	from usr in twitterCtx.TwitterUser
		//	where usr.Type == UserType.Me
		//	select usr
		//).SingleOrDefaultAsync();

		return Ok(authString);
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

	//[HttpPost]
	//public async Task<ActionResult<L2TBase>> UserInfo(L2TBase l2tBase)
	//{
	//	OAuth2Authorizer auth = L2TOAuth2(l2tBase.AccessToken, l2tBase.RefreshToken);
	//	TwitterContext twitterCtx = new TwitterContext(auth); // #TODO Try/Catch

	//	TwitterUserQuery response = await (
	//		from usr in twitterCtx.TwitterUser
	//		where usr.Type == UserType.Me
	//		select usr
	//	).SingleOrDefaultAsync();

	//	TwitterUser user = response?.Users?.SingleOrDefault();

	//	l2tBase.UserId = Convert.ToInt64(user.ID);

	//	return Ok(l2tBase);
	//}

}

//// TEST CODE

////
//// OLD Media List
////
//Media = (from tweetMedia in tweetMediaList
//		 select new L2TTwitterMediaDTO()
//		 {
//			 MediaKey = tweetMedia.MediaKey,
//			 Type = tweetMedia.Type,
//			 PreviewImageUrl = tweetMedia.PreviewImageUrl,
//			 AltText = tweetMedia.AltText,
//			 Width = tweetMedia.Width,
//			 Height = tweetMedia.Height,
//			 DurationMS = tweetMedia.DurationMS,
//			 Url = tweetMedia.Url,
//		 }
//		 ).Where(media => tweet.Attachments.MediaKeys.Contains(media.MediaKey)).ToList()

////
//// EVEN OLDER Media List
////
//Media = (from mediaKey in tweet.Attachments.MediaKeys
//		 select new L2TTwitterMediaDTO
//		 {
//			 MediaKey = mediaKey,
//			 //Type = tweetQuery.Includes.Media.Where(key => key.MediaKey == mediaKey).FirstOrDefault().Type.ToString(),
//			 Url = tweetQuery.Includes.Media.Where(key => key.MediaKey == mediaKey).FirstOrDefault().Url.ToString(),
//			 PreviewImageUrl = tweetQuery.Includes.Media.Where(key => key.MediaKey == mediaKey).FirstOrDefault().PreviewImageUrl?.ToString(),
//			 AltText = tweetQuery.Includes.Media.Where(key => key.MediaKey == mediaKey).FirstOrDefault().AltText?.ToString(),
//			 Width = tweetQuery.Includes.Media.Where(key => key.MediaKey == mediaKey).FirstOrDefault().Width,
//			 Height = tweetQuery.Includes.Media.Where(key => key.MediaKey == mediaKey).FirstOrDefault().Height,
//			 DurationMS = tweetQuery.Includes.Media.Where(key => key.MediaKey == mediaKey).FirstOrDefault().DurationMS,
//		 }).ToList(),



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