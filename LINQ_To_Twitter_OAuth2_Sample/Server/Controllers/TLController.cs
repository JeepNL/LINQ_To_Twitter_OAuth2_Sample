using LINQ_To_Twitter_OAuth2_Sample.Server.Helpers;
using LINQ_To_Twitter_OAuth2_Sample.Shared.Models;
using LinqToTwitter;
using LinqToTwitter.Common;
using LinqToTwitter.OAuth;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace LINQ_To_Twitter_OAuth2_Sample.Server.Controllers;

[Route("[controller]/[action]")]
[ApiController]

public class TLController : ControllerBase
{
	[HttpPost]
	public async Task<ActionResult<List<L2TTimelineResponse>>> UserTimeline(L2TTimelineRequest l2tTimelineRequest)
	{
		OAuth2Authorizer auth = OAuth2Helper.Authorizer(l2tTimelineRequest.AccessToken!, l2tTimelineRequest.RefreshToken!);

		TwitterContext twitterCtx = new TwitterContext(auth); // #TODO Try/Catch

		TweetQuery? tweetQuery = await (
				from tweet in twitterCtx.Tweets
				where tweet.Type == TweetType.TweetsTimeline
					&& tweet.ID == l2tTimelineRequest.ForUserId.ToString() // tweet.ID = Tweet AuthorId, not the id of the tweet.
					&& tweet.UserFields == $"{UserField.AllFields}"
					&& tweet.TweetFields == $"{TweetField.AllFieldsExceptPermissioned}"
					&& tweet.Expansions == $"{ExpansionField.MediaKeys},{ExpansionField.AuthorID}, {ExpansionField.ReferencedTweetID}"
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
			TwitterUser? author = tweetQuery.Includes?.Users?.Where(twitterUser => twitterUser.ID == l2tTimelineRequest.ForUserId.ToString()).FirstOrDefault();

			List<L2TTimelineResponse> utlResponse = (
					from tweet
					in tweetQuery.Tweets
					select new L2TTimelineResponse // = DTO
					{
						TweetId = tweet.ID,
						ScreenName = author?.Username,
						Name = author?.Name,
						AuthorId = author?.ID,
						ProfileImageUrl = author?.ProfileImageUrl,
						Text = tweet.Text!.Replace("\n", "<br />"),
						TweetDate = tweet.CreatedAt,
						Source = tweet.Source,

						// if tweet.RefecencedTweets.Type == "retweeted" full text of retweeted tweet is in includedTweets.Text
						ReferencedTweets = tweet.ReferencedTweets is null ? null : (
							from includedTweets
							in tweetQuery.Includes?.Tweets
							where tweet.ReferencedTweets
								.Select(refTweet => refTweet.ID)
								.Contains(includedTweets.ID)
							select new L2TReferencedTweetDTO
							{
								Id = includedTweets.ID,
								Text = includedTweets.Text?.Replace("\n", "<br />"),
								Type = tweet.ReferencedTweets?
									.Where(refTweet => refTweet.ID == includedTweets.ID)
									.Select(a => a.Type).SingleOrDefault()?
									.ToString()
							}
						).ToArray(),

						// #TODO MENTIONS & HASHTAGS

						Urls = tweet.Entities?.Urls is null ? null : (
							from selectEntityUrl
							in tweet.Entities?.Urls
							select new L2TTweetEntityUrlDTO
							{
								Url = selectEntityUrl.Url,
								DisplayUrl = selectEntityUrl.DisplayUrl,
								ExpandedUrl = selectEntityUrl.ExpandedUrl,
							}
						).ToArray(),

						Media = tweetQuery.Includes?.Media is null ? null : (
							from mediaKey
							in tweetQuery.Includes?.Media
							where (tweet.Attachments?.MediaKeys is null) ? false : tweet.Attachments.MediaKeys.Contains(mediaKey.MediaKey!)
							select new L2TTwitterMediaDTO
							{
								MediaKey = mediaKey.MediaKey,
								Type = (L2TTweetMediaType)mediaKey.Type,
								PreviewImageUrl = mediaKey.PreviewImageUrl,
								AltText = mediaKey.AltText,
								Width = mediaKey.Width,
								Height = mediaKey.Height,
								DurationMS = mediaKey.DurationMS,
								Url = mediaKey.Url,
							}
						).ToArray(),
					}
			).ToList();

			return Ok(utlResponse);
		}
		else
			return NotFound(); // #TODO, isn't 'NotFound', but 'No Results' ...
	}
}
