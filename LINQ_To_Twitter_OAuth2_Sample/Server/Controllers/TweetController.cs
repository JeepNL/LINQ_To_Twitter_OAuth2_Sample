using LINQ_To_Twitter_OAuth2_Sample.Server.Helpers;
using LINQ_To_Twitter_OAuth2_Sample.Shared.Models;
using LinqToTwitter;
using LinqToTwitter.OAuth;
using Microsoft.AspNetCore.Mvc;

namespace LINQ_To_Twitter_OAuth2_Sample.Server.Controllers;

[Route("[controller]/[action]")]
[ApiController]

public class TweetController : ControllerBase
{
	private readonly ILogger<OAuth2Controller> _logger;

	public TweetController(ILogger<OAuth2Controller> logger)
	{
		_logger = logger;
	}

	[HttpPost]
	public async Task<ActionResult<L2TTweet>> Post(L2TTweet l2tTweet)
	{
		if (!string.IsNullOrEmpty(l2tTweet.Text))
		{
			OAuth2Authorizer auth = OAuth2Helper.Authorizer(l2tTweet.AccessToken!, l2tTweet.RefreshToken!);
			TwitterContext twitterCtx = new TwitterContext(auth); // #TODO Try/Catch
			Tweet? tweet = new();

			try
			{
				tweet = await twitterCtx.TweetAsync(l2tTweet.Text);
				l2tTweet.TweetId = tweet?.ID!;
			}
			catch (Exception e)
			{
				_logger.LogError($"***** PostTweet e.StackTrace: {e.StackTrace}");
				l2tTweet.ErrorMessage = e.Message;
				l2tTweet.TweetId = "-1";
				return BadRequest(l2tTweet);
			}
		}
		return Ok(l2tTweet);
	}

	[HttpPost]
	public async Task<ActionResult> Delete(L2TTweet l2tTweet)
	{
		if (!string.IsNullOrEmpty(l2tTweet.TweetId) && !string.IsNullOrEmpty(l2tTweet.AuthorId))
		{
			OAuth2Authorizer auth = OAuth2Helper.Authorizer(l2tTweet.AccessToken!, l2tTweet.RefreshToken!);
			TwitterContext twitterCtx = new TwitterContext(auth);

			try
			{
				await twitterCtx.DeleteTweetAsync(l2tTweet.TweetId);
				return Ok();
			}
			catch (Exception e)
			{
				_logger.LogError($"***** PostTweet e.StackTrace: {e.StackTrace}");
				l2tTweet.ErrorMessage = e.Message;
				l2tTweet.TweetId = "-1";
				return BadRequest(l2tTweet);
			}
		}
		else
			return NotFound();
	}
}

//Console.WriteLine($"***** : {}");