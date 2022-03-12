using LINQ_To_Twitter_OAuth2_Sample.Server.Helpers;
using LINQ_To_Twitter_OAuth2_Sample.Shared.Models;
using LinqToTwitter;
using LinqToTwitter.Common;
using LinqToTwitter.OAuth;
using Microsoft.AspNetCore.Mvc;

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
					&& tweet.Expansions == $"{ExpansionField.MediaKeys},{ExpansionField.AuthorID},{ExpansionField.PlaceID}"
					&& tweet.MediaFields == $"{MediaField.AllFieldsExceptPermissioned}"
					&& tweet.PlaceFields == $"{PlaceField.AllFields}"
					&& tweet.MaxResults == l2tTimelineRequest.MaxResults // default = 10
					&& tweet.SinceID == l2tTimelineRequest.SinceId.ToString() // default = 0
				select tweet
		).SingleOrDefaultAsync();

		if (tweetQuery is not null)
		{
			// TweetType.TweetsTimeline has only 1 author
			// tweetQuery.Includes.Users works because of ExpansionField.AuthorID
			TwitterUser? author = tweetQuery.Includes?.Users?.Where(twitterUser => twitterUser.ID == l2tTimelineRequest.ForUserId.ToString()).FirstOrDefault();

			// One query for the complete media list in tweetQuery - for: NEW Media List below
			List<L2TTwitterMediaDTO>? tweetMediaList = tweetQuery.Includes?.Media is null ? null : (
				from mediaKey
				in tweetQuery.Includes?.Media
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
					// #TODO (?)
					//PublicMetrics = mediaKey.PublicMetrics
					//NonPublicMetrics = mediaKey.NonPublicMetrics,
					//OrganicMetrics = mediaKey.OrganicMetrics,
					//PromotedMetrics = mediaKey.PromotedMetrics,
				}
			).ToList();

			List<L2TTimelineResponse> utlResponse = (
					from tweet
					in tweetQuery.Tweets
					select new L2TTimelineResponse // = DTO
					{
						//TweetId = Convert.ToInt64(tweet.ID),
						TweetId = tweet.ID,
						ScreenName = author?.Username,
						Name = author?.Name,
						AuthorId = author?.ID,
						ProfileImageUrl = author?.ProfileImageUrl,
						Text = tweet.Text?.Replace("\n", "<br />"),
						TweetDate = tweet.CreatedAt,
						Source = tweet.Source,

						// #TODO MENTIONS * HASHTAGS

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

						// NEW Media List // works but doesn't look/feel good to me (yet) i.e: from selectTweetMedia / select selectTweetMedia ...
						//Media = tweetMediaList?.Count is null ? null : (
						Media = tweetMediaList is null ? null : (
							from selectTweetMedia
							in tweetMediaList
							where (tweet.Attachments?.MediaKeys is null) ? false : tweet.Attachments.MediaKeys.Contains(selectTweetMedia.MediaKey!)
							//where tweet.Attachments?.MediaKeys is not null && tweet.Attachments.MediaKeys.Contains(selectTweetMedia.MediaKey)
							select selectTweetMedia
						).ToArray()
					}
			).ToList();

			return Ok(utlResponse);
		}
		else
			return NotFound(); // #TODO, isn't 'NotFound', but 'No Results' ...
	}
}
