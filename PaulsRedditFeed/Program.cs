using PaulsRedditFeed;
using Reddit;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorPages();
builder.Services.AddSignalR(options =>
{
    options.EnableDetailedErrors = true;
    options.KeepAliveInterval = TimeSpan.FromMinutes(1);
});
var settings = builder.Configuration.GetSection("AppSettings").Get<AppSettings>();
if (settings?.Reddit.ApiKey == null)
{
    var message = "Unable to find AppSettings section from app configuration. Check appsettings.json and secrets configuration.";
    throw new SettingsLoadException(message);
}

// Add services to the container.
builder.Services.AddSingleton(settings);
builder.Services.AddSingleton(settings.Reddit);
builder.Services.AddControllersWithViews();
builder.Services.AddLogging(loggers => loggers.AddConsole());
builder.Services.AddControllersWithViews();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddHttpClient<RedditMonitor>()
    .ConfigureHttpClient(httpClient =>
    {
        httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer ${settings.Reddit.ApiKey}");
        httpClient.DefaultRequestHeaders.Add("X-Modhash", settings.Reddit.ModHash);
        httpClient.BaseAddress = new Uri("https://www.reddit.com/r/search/");
    });
builder.Services.AddHostedService<RedditMonitor>();
builder.Services.AddSingleton<FilterManager>();

var redditClient = new RedditClient(settings.Reddit.AppId, settings.Reddit.RefreshToken, settings.Reddit.ApiKey);
builder.Services.AddSingleton((services) => redditClient);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapHub<RedditStatsHub>("/stats");
app.Run();
