using Microsoft.AspNetCore.Authentication.JwtBearer;
using PaulsRedditFeed;
using Reddit;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("appsettings.Testing.json", optional: true, reloadOnChange: true);
var settings = builder.Configuration.GetSection("AppSettings").Get<AppSettings>();
if (settings == null)
{
    var message = "Unable to find AppSettings section from app configuration. Check appsettings.json and secrets configuration.";
    throw new SettingsLoadException(message);
}

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();

builder.Services.AddSignalR().AddStackExchangeRedis(settings.Redis.ConnectionString, options =>
{
    /* Enable sticky sessions. Redis is used as the backplane to keep users connected to
     * the same server when communicating over websockets.
     */
    options.Configuration.ChannelPrefix = "PaulsRedditFeed";
});
builder.Services.AddSingleton(settings);
builder.Services.AddControllersWithViews();
builder.Services.AddLogging(loggers => loggers.AddConsole());
builder.Services.AddControllersWithViews();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddHttpClient<RedditMonitor>()
    .ConfigureHttpClient(httpClient =>
    {
        httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {settings.Reddit.RefreshToken}");
        httpClient.BaseAddress = new Uri(settings.Reddit.BaseUrl);
    });
builder.Services.AddHostedService<RedditMonitor>();
builder.Services.AddHostedService<RedditStatsProcessor>();
builder.Services.AddSingleton<FilterManager>();
builder.Services.AddSingleton(ConnectionMultiplexer.Connect(settings.Redis.ConnectionString));

var redditClient = new RedditClient(
    settings.Reddit.AppId,
    settings.Reddit.RefreshToken,
    settings.Reddit.AppSecret,
    userAgent: "windows:lionsandtigersubzilla:v2.0.0 (by /u/lionsandtigersubzilla)");
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
