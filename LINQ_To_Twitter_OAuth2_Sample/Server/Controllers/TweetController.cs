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
	private readonly ILogger<TweetController> _logger;
	private readonly IWebHostEnvironment _webHostEnvironment;

	public TweetController(ILogger<TweetController> logger, IWebHostEnvironment webHostEnvironment)
	{
		_logger = logger;
		_webHostEnvironment = webHostEnvironment;
	}

	[HttpPost] // #TODO SECURE API ENDPOINTS
	public async Task<ActionResult<L2TTweet>> Post(L2TTweet l2tTweet)
	{
		if (string.IsNullOrEmpty(l2tTweet.AccessToken) || string.IsNullOrEmpty(l2tTweet.RefreshToken) || string.IsNullOrEmpty(l2tTweet.Text))
			return BadRequest("AccessToken, RefreshToken and Text are required.");

		OAuth2Authorizer auth = OAuth2Helper.Authorizer(l2tTweet.AccessToken, l2tTweet.RefreshToken);
		TwitterContext twitterCtx = new TwitterContext(auth);

		Tweet? tweet = new();
		string mediaCategory = "tweet_image";

		if (l2tTweet.ImagesB64 is not null)
		{
			Console.WriteLine($"***** l2tTweet.ImagesB64 is not null");

			foreach (var imgUpload in l2tTweet.ImagesB64)
			{
				Console.WriteLine($"***** foreach (var imgUpload in l2tTweet.ImagesB64)");
				byte[] imageBytes = Convert.FromBase64String(imgUpload.Data!);

				//usiing var image = SixLabors.ImageSharp.Image.Load(imageBytes);
				//var image2 = image.CloneAs();

				string imageFormat = imgUpload.Format!;

				string uploadsFolder = Path.Combine(_webHostEnvironment.ContentRootPath, "Upload", "Images");
				string filePath = Path.Combine(uploadsFolder, "FNL3hb8XsAIwyOC.jpg");
				Console.WriteLine($"***** filePath: {filePath}");

				//var testImageBytes = System.IO.File.ReadAllBytes(filePath);

				Media? media = await twitterCtx.UploadMediaAsync(System.IO.File.ReadAllBytes(filePath), "image/jpg", mediaCategory);

				if (media == null)
				{
					Console.WriteLine("Problem uploading media.");
					return BadRequest("Problem uploading media.");
				}

				tweet = await twitterCtx.TweetMediaAsync(l2tTweet.Text, new List<string> { media.MediaID.ToString() });
				//Tweet? twt = await twitterCtx.TweetMediaAsync(l2tTweet.Text, new List<string> { media.MediaID.ToString() });

				if (tweet != null)
					Console.WriteLine("Tweet sent");
				else
					Console.WriteLine("Tweet not sent");

				//List<Task<Media?>> imageUploadTasks = new()
				//	{
				//		//twitterCtx.UploadMediaAsync(testImageBytes, $"image/{imageFormat}", mediaCategory),
				//		twitterCtx.UploadMediaAsync(System.IO.File.ReadAllBytes(filePath), $"image/jpg", mediaCategory),
				//	};
				//await Task.WhenAll(imageUploadTasks);

				//Console.WriteLine($"***** [AFTER] await Task.WhenAll(imageUploadTasks);");

				//List<string> mediaIds =
				//	(from tsk in imageUploadTasks
				//	 select tsk.Result.MediaID.ToString())
				//	.ToList();

				//Console.WriteLine($"***** [AFTER] List<string> mediaIds =");

				//try
				//{
				//	tweet = await twitterCtx.TweetMediaAsync(l2tTweet.Text, mediaIds);
				//}
				//catch (Exception ex)
				//{
				//	_logger.LogError($"***** PostTweet e.StackTrace: {ex.StackTrace}");
				//	l2tTweet.ErrorMessage = ex.Message;
				//	l2tTweet.TweetId = "-1";
				//	return BadRequest(l2tTweet);
				//}
			}
		}
		else
		{
			try
			{
				tweet = await twitterCtx.TweetAsync(l2tTweet.Text);
			}
			catch (Exception ex)
			{
				_logger.LogError($"***** PostTweet e.StackTrace: {ex.StackTrace}");
				l2tTweet.ErrorMessage = ex.Message;
				l2tTweet.TweetId = "-1";
				return BadRequest(l2tTweet);
			}
		}

		l2tTweet.TweetId = tweet?.ID;
		return Ok(l2tTweet);
	}

	[HttpPost]
	public async Task<ActionResult> Delete(L2TTweet l2tTweet) // MAX 50 per 15 minutes
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