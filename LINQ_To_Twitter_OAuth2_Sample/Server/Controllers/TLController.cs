using LINQ_To_Twitter_OAuth2_Sample.Server.Helpers;
using LINQ_To_Twitter_OAuth2_Sample.Shared.Models;
using LinqToTwitter;
using LinqToTwitter.Common;
using LinqToTwitter.OAuth;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Text;
using System.Text.Json;

namespace LINQ_To_Twitter_OAuth2_Sample.Server.Controllers;

[Route("[controller]/[action]")]
[ApiController]

public class TLController : ControllerBase
{
	[HttpPost]
	public async Task<ActionResult<string>> UserTimeline(L2TTimelineRequest l2tTimelineRequest)
	{
		OAuth2Authorizer auth = OAuth2Helper.Authorizer(l2tTimelineRequest.AccessToken!, l2tTimelineRequest.RefreshToken!);
		TwitterContext twitterCtx = new TwitterContext(auth); // #TODO Try/Catch

		TweetQuery? tweetQuery = await (
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

		string jsonResult = string.Empty;
		if (tweetQuery is not null)
		{
			var options = new JsonSerializerOptions
			{
				WriteIndented = false,
			};
			jsonResult = JsonSerializer.Serialize(tweetQuery, options);
		}
		else
			return NotFound();

		return Ok(jsonResult);
	}

}

//Console.WriteLine("\n\n");
//Console.WriteLine($"***** : {}");
//Console.WriteLine("\n\n");
