using PaulsRedditFeed;
using PaulsRedditFeed.Services;
using Reddit;

var builder = WebApplication.CreateBuilder(args);
var settings = builder.Configuration.GetSection("AppSettings").Get<AppSettings>();
if (settings?.Reddit.ApiKey == null)
{
    var message = "Unable to find AppSettings section from app configuration. Check appsettings.json and secrets configuration.";
    throw new SettingsLoadException(message);
}

// Add services to the container.
builder.Services.AddLogging(loggers => loggers.AddConsole());
builder.Services.AddControllersWithViews();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddHttpClient<RedditFeed>()
    .ConfigureHttpClient(httpClient =>
    {
        httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer ${settings.Reddit.ApiKey}");
        httpClient.DefaultRequestHeaders.Add("X-Modhash", settings.Reddit.ModHash);
        httpClient.BaseAddress = new Uri("https://www.reddit.com/r/search/");
    });
builder.Services.AddHostedService<RedditFeed>();
builder.Services.AddSingleton(settings);
builder.Services.AddSingleton<FilterManager>();
builder.Services.AddSignalR();

var redditClient = new RedditClient(settings.Reddit.AppId, settings.Reddit.RefreshToken, settings.Reddit.ApiKey);
var thing = redditClient.Account.Me.Name;
builder.Services.AddSingleton((services) => redditClient);

var app = builder.Build();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action=Index}/{id?}");

app.MapFallbackToFile("index.html");

app.Run();
