using Blazored.LocalStorage;
using LINQ_To_Twitter_OAuth2_Sample.Client;
using LINQ_To_Twitter_OAuth2_Sample.Client.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddSingleton<AppDataService>();

builder.Services.AddBlazoredLocalStorage();

await builder.Build().RunAsync();
