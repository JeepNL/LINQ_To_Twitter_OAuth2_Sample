using LinqToTwitter;
using LinqToTwitter.OAuth;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;

namespace LINQ_To_Twitter_OAuth2_Sample.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class OAuth2Controller : ControllerBase
{
	private readonly ILogger<OAuth2Controller> _logger;
	private readonly IConfiguration _configuration;

	public OAuth2Controller(ILogger<OAuth2Controller> logger, IConfiguration configuration)
	{
		_logger = logger;
		_configuration = configuration;
	}

	//public ActionResult Index()
	//{
	//	return View();
	//}

	[HttpGet("BeginAsync")]
	public async Task<ActionResult> BeginAsync()
	{
		string twitterCallbackUrl = Request.GetDisplayUrl().Replace("Begin", "Complete");

		Console.WriteLine($"\n\n***** TwitterClientID: {_configuration["TwitterClientID"]}");
		Console.WriteLine($"***** TwitterClientSecret: {_configuration["TwitterClientSecret"]}");
		Console.WriteLine($"***** twitterCallbackUrl: {twitterCallbackUrl}\n\n");

		var auth = new MvcOAuth2Authorizer
		{
			CredentialStore = new OAuth2SessionCredentialStore(HttpContext.Session)
			{
				ClientID = _configuration["TwitterClientID"],
				ClientSecret = _configuration["TwitterClientSecret"],
				//ClientID = Environment.GetEnvironmentVariable(OAuthKeys.TwitterClientID),
				//ClientSecret = Environment.GetEnvironmentVariable(OAuthKeys.TwitterClientSecret),
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

	[HttpGet("CompleteAsync")]
	public async Task<ActionResult> CompleteAsync()
	{
		var auth = new MvcOAuth2Authorizer
		{
			CredentialStore = new OAuth2SessionCredentialStore(HttpContext.Session)
		};

		Request.Query.TryGetValue("code", out StringValues code);
		Request.Query.TryGetValue("state", out StringValues state);

		Console.WriteLine($"\n\n***** code: {code}");
		Console.WriteLine($"***** state: {state}\n\n");

		await auth.CompleteAuthorizeAsync(code, state);

		//RedirectToAction("Index", "Home");

		return Redirect("/"); // #TODO
	}
}
