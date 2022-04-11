var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSystemd();

// read twitterkeys.ini file, set environment variables.
builder.Host.ConfigureAppConfiguration((hostingContext, config) =>
{
	// see: https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/?view=aspnetcore-6.0#ini-configuration-provider
	config.Sources.Clear();
	var env = hostingContext.HostingEnvironment;
	config.AddIniFile("twitterkeys.ini", optional: true, reloadOnChange: true);
	//.AddIniFile($"twitterkeys.{env.EnvironmentName}.ini", optional: true, reloadOnChange: true);
	config.AddEnvironmentVariables();
});

// Add services to the container.
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
	options.Cookie.HttpOnly = false;
	options.Cookie.IsEssential = true;
	options.IdleTimeout = TimeSpan.FromMinutes(30);
	options.Cookie.Name = "L2T.Session";
});
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseWebAssemblyDebugging();
}
else
{
	app.UseExceptionHandler("/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseHttpsRedirection();

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();

//app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();
