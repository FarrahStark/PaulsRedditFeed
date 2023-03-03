using Microsoft.AspNetCore.Authentication.JwtBearer;
using PaulsRedditFeed;
using System.Net;
using System.Net.Http;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables("REDDITFEED");
var settings = builder.Configuration.GetSection("AppSettings").Get<AppSettings>();
if (settings == null)
{
    var message = "Unable to find AppSettings section from app configuration. Check appsettings.json and secrets configuration.";
    throw new SettingsLoadException(message);
}

builder.Services.AddRequestDecompression();
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

builder.Services.AddTransient<RedditTokenHandler>();
builder.Services.AddTransient<RedditApiClient>();

// Creates an Http Client for requesting date from the reddit API
builder.Services.AddHttpClient(RedditTokenHandler.SearchClientName, httpClient =>
    {
        httpClient.BaseAddress = new Uri(settings.Reddit.BaseUrl);
        httpClient.DefaultRequestHeaders.Add("Accept", "application/json, text/json, text/x-json, text/javascript, application/xml, text/xml");
        httpClient.DefaultRequestHeaders.Add("User-Agent", settings.Reddit.UserAgent);
        httpClient.DefaultRequestHeaders.Add("Connection", "Keep-Alive");
        httpClient.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate");
    })
    .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
    {
        AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
    })
    .AddHttpMessageHandler<RedditTokenHandler>(); // handles OAuth2.0 access token management for the reddit API

// Register Http client for refreshing access tokens as needed to maintain
// Authentication with OAuth2, otherwise the Acces token will expire and API request will fail
builder.Services.AddHttpClient(RedditTokenHandler.AuthClientName, client =>
    {
        client.BaseAddress = new Uri(settings.Reddit.AuthUrl);
        client.DefaultRequestHeaders.Add("Accept", "application/json");
    });

builder.Services.AddHostedService<RedditMonitor>();
builder.Services.AddHostedService<RedditStatsProcessor>();
builder.Services.AddSingleton<FilterManager>();
builder.Services.AddSingleton(ConnectionMultiplexer.Connect(settings.Redis.ConnectionString));

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
app.UseRequestDecompression();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapHub<RedditStatsHub>("/stats");
app.Run();
