using LINQ_To_Twitter_OAuth2_Sample.Server.Helpers;
using LINQ_To_Twitter_OAuth2_Sample.Shared.Models;
using LinqToTwitter;
using LinqToTwitter.Common;
using LinqToTwitter.OAuth;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace LINQ_To_Twitter_OAuth2_Sample.Server.Controllers;

[Route("[controller]/[action]")]
[ApiController]

public class TLController : ControllerBase
{
	private readonly ILogger<TLController> _logger;
	public TLController(ILogger<TLController> logger)
	{
		_logger = logger;
	}

	[HttpPost] // #TODO SECURE API ENDPOINTS
	public async Task<ActionResult<string>> UserTimeline(L2TTimelineRequest l2tTimelineRequest)
	{
		if (string.IsNullOrEmpty(l2tTimelineRequest.AccessToken) || string.IsNullOrEmpty(l2tTimelineRequest.RefreshToken))
			return BadRequest("AccessToken and RefreshToken are required.");

		OAuth2Authorizer auth = OAuth2Helper.Authorizer(l2tTimelineRequest.AccessToken!, l2tTimelineRequest.RefreshToken!);
		TwitterContext twitterCtx = new(auth);

		TweetQuery? tweetQuery;
		try
		{
			tweetQuery = await (
					from tweet in twitterCtx.Tweets
					where tweet.Type == TweetType.TweetsTimeline
						&& tweet.Expansions ==
							$"{ExpansionField.MediaKeys}," +
							$"{ExpansionField.AuthorID}," +
							$"{ExpansionField.ReferencedTweetID}," +
							$"{ExpansionField.ReferencedTweetAuthorID}," +
							$"{ExpansionField.MentionsUsername}," +
							$"{ExpansionField.InReplyToUserID}," +
							$"{ExpansionField.PollIds}," +
							$"{ExpansionField.PlaceID}"
						&& tweet.ID == l2tTimelineRequest.ForUserId.ToString() // tweet.ID = Tweet AuthorId, not the id of the tweet.
						&& tweet.UserFields == $"{UserField.AllFields}"
						&& tweet.TweetFields == $"{TweetField.AllFieldsExceptPermissioned}"
						&& tweet.MediaFields == $"{MediaField.AllFieldsExceptPermissioned}"
						&& tweet.PlaceFields == $"{PlaceField.AllFields}"
						&& tweet.PollFields == $"{PollField.AllFields}"
						&& tweet.MaxResults == l2tTimelineRequest.MaxResults // default = 10
					select tweet
			).SingleOrDefaultAsync();
		}
		catch (Exception ex) // #TODO Authorization failed? If so, log IP address etc. Prohibit any further calls to API (for time period?)
		{
			_logger.LogError($"***** PostTweet e.StackTrace: {ex.StackTrace}");
			return BadRequest(ex.Message);
		}


		if (tweetQuery is null)
			return NotFound("No tweets found.");

		return Ok(JsonSerializer.Serialize(tweetQuery, new JsonSerializerOptions { WriteIndented = false }));
	}
}

//Console.WriteLine("\n\n");
//Console.WriteLine($"***** : {}");
//Console.WriteLine("\n\n");
