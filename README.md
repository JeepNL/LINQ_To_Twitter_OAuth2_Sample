Kestrel Hosted Blazor WASM / LINQ To Twitter OAuth2 Sample

Joe Mayo's Linq To Twitter OAuth2 blog post: https://joemayo.medium.com/using-oauth-2-0-with-linq-to-twitter-eac6d9035084

For this sample to work you'll need to add the file `/Server/twitterkeys.ini` which is called in `/Server/Program.cs` (line 5-13) and sets the `TwitterClientID` & `TwitterClientSecret` enviroment variables.

    TwitterClientID="Your Twitter Client ID"
    TwitterClientSecret="Your Twitter Client Secret"

(*You can obtain your Twitter OAuth2 Client ID & Client Secret at https://developer.twitter.com/*)

This sample is using `https://localhost` (*not 127.0.0.1*) for a valid localhost client certificate, so the Twitter callback url is: `https://localhost/OAuth2/CompleteAsync`

This app is configured at developer.twitter.com as a **Web App**, not a Single Page App. Twitter Description: "Web App uses confidential clients, which securely authenticate with the authorization server. They keep your client secret safe."

**Notable files:**

*Client:*

 * [Client/Pages/LinqToTwitter.razor](https://github.com/JeepNL/LINQ_To_Twitter_OAuth2_Sample/blob/master/LINQ_To_Twitter_OAuth2_Sample/Client/Pages/Linq2Twitter.razor)

*Server*

 * [Server/Program.cs](https://github.com/JeepNL/LINQ_To_Twitter_OAuth2_Sample/tree/master/LINQ_To_Twitter_OAuth2_Sample/Server/Program.cs)
 * [Server/Controllers/OAuth2Controller.cs](https://github.com/JeepNL/LINQ_To_Twitter_OAuth2_Sample/tree/master/LINQ_To_Twitter_OAuth2_Sample/Server/Controllers/OAuth2Controller.cs)

Error log (`Server/Controllers/OAuth2Controller.cs`) at **CompleteAsync** `https://localhost/OAuth2/CompleteAsync?state=MyState&code=<RETURNED_TWITTER_CODE>`:

**TwitterQueryException: Missing valid authorization header - Please visit the LINQ to Twitter FAQ (at the HelpLink) for help on resolving this error.**

    info: Microsoft.AspNetCore.Mvc.Infrastructure.ControllerActionInvoker[2]
          Executed action LINQ_To_Twitter_OAuth2_Sample.Server.Controllers.OAuth2Controller.CompleteAsync (LINQ_To_Twitter_OAuth2_Sample.Server) in 292.2686ms
    info: Microsoft.AspNetCore.Routing.EndpointMiddleware[1]
          Executed endpoint 'LINQ_To_Twitter_OAuth2_Sample.Server.Controllers.OAuth2Controller.CompleteAsync (LINQ_To_Twitter_OAuth2_Sample.Server)'
    fail: Microsoft.AspNetCore.Diagnostics.DeveloperExceptionPageMiddleware[1]
          An unhandled exception has occurred while executing the request.
          LinqToTwitter.Common.TwitterQueryException: Missing valid authorization header - Please visit the LINQ to Twitter FAQ (at the HelpLink) for help on resolving this error.
             at LinqToTwitter.Net.TwitterErrorHandler.HandleUnauthorizedAsync(HttpResponseMessage msg)
             at LinqToTwitter.Net.TwitterErrorHandler.ThrowIfErrorAsync(HttpResponseMessage msg)
             at LinqToTwitter.OAuth.OAuth2Authorizer.SendHttpAsync(HttpRequestMessage req)
             at LinqToTwitter.OAuth.OAuth2Authorizer.GetAccessTokenAsync(String code)
             at LinqToTwitter.OAuth.OAuth2Authorizer.CompleteAuthorizeAsync(String code, String state)
             at LINQ_To_Twitter_OAuth2_Sample.Server.Controllers.OAuth2Controller.CompleteAsync() in D:\Repos\LINQ_To_Twitter_OAuth2_Sample\LINQ_To_Twitter_OAuth2_Sample\Server\Controllers\OAuth2Controller.cs:line 84
             at lambda_method12(Closure , Object )
             at Microsoft.AspNetCore.Mvc.Infrastructure.ActionMethodExecutor.TaskOfActionResultExecutor.Execute(IActionResultTypeMapper mapper, ObjectMethodExecutor executor, Object controller, Object[] arguments)
             at Microsoft.AspNetCore.Mvc.Infrastructure.ControllerActionInvoker.<InvokeActionMethodAsync>g__Awaited|12_0(ControllerActionInvoker invoker, ValueTask`1 actionResultValueTask)
             at Microsoft.AspNetCore.Mvc.Infrastructure.ControllerActionInvoker.<InvokeNextActionFilterAsync>g__Awaited|10_0(ControllerActionInvoker invoker, Task lastTask, State next, Scope scope, Object state, Boolean isCompleted)
             at Microsoft.AspNetCore.Mvc.Infrastructure.ControllerActionInvoker.Rethrow(ActionExecutedContextSealed context)
             at Microsoft.AspNetCore.Mvc.Infrastructure.ControllerActionInvoker.Next(State& next, Scope& scope, Object& state, Boolean& isCompleted)
             at Microsoft.AspNetCore.Mvc.Infrastructure.ControllerActionInvoker.<InvokeInnerFilterAsync>g__Awaited|13_0(ControllerActionInvoker invoker, Task lastTask, State next, Scope scope, Object state, Boolean isCompleted)
             at Microsoft.AspNetCore.Mvc.Infrastructure.ResourceInvoker.<InvokeNextResourceFilter>g__Awaited|25_0(ResourceInvoker invoker, Task lastTask, State next, Scope scope, Object state, Boolean isCompleted)
             at Microsoft.AspNetCore.Mvc.Infrastructure.ResourceInvoker.Rethrow(ResourceExecutedContextSealed context)
             at Microsoft.AspNetCore.Mvc.Infrastructure.ResourceInvoker.Next(State& next, Scope& scope, Object& state, Boolean& isCompleted)
             at Microsoft.AspNetCore.Mvc.Infrastructure.ResourceInvoker.<InvokeFilterPipelineAsync>g__Awaited|20_0(ResourceInvoker invoker, Task lastTask, State next, Scope scope, Object state, Boolean isCompleted)
             at Microsoft.AspNetCore.Mvc.Infrastructure.ResourceInvoker.<InvokeAsync>g__Logged|17_1(ResourceInvoker invoker)
             at Microsoft.AspNetCore.Mvc.Infrastructure.ResourceInvoker.<InvokeAsync>g__Logged|17_1(ResourceInvoker invoker)
             at Microsoft.AspNetCore.Routing.EndpointMiddleware.<Invoke>g__AwaitRequestTask|6_0(Endpoint endpoint, Task requestTask, ILogger logger)
             at Microsoft.AspNetCore.Session.SessionMiddleware.Invoke(HttpContext context)
             at Microsoft.AspNetCore.Session.SessionMiddleware.Invoke(HttpContext context)
             at Microsoft.AspNetCore.Authorization.AuthorizationMiddleware.Invoke(HttpContext context)
             at Microsoft.AspNetCore.Diagnostics.DeveloperExceptionPageMiddleware.Invoke(HttpContext context)
