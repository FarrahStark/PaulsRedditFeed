using Microsoft.AspNetCore.Authentication.JwtBearer;
using PaulsRedditFeed;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables("REDDITFEED");
var settings = builder.Configuration.GetSection("AppSettings").Get<AppSettings>();
if (settings == null)
{
    var message = "Unable to find AppSettings section from app configuration. Check appsettings.json and secrets configuration.";
    throw new SettingsLoadException(message);
}

// Add services to the container.
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();
builder.Services.AddRequestDecompression();

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
builder.Services.AddHttpClient(RedditTokenHandler.SearchClientName, httpClient =>
    {
        httpClient.BaseAddress = new Uri(settings.Reddit.BaseUrl);
        httpClient.DefaultRequestHeaders.Add("Accept", "application/json, text/json, text/x-json, text/javascript, application/xml, text/xml");
        httpClient.DefaultRequestHeaders.Add("User-Agent", settings.Reddit.UserAgent);
        httpClient.DefaultRequestHeaders.Add("Connection", "Keep-Alive");
        httpClient.DefaultRequestHeaders.Add("Accept-Encoding", "gzip,deflate");
    }).AddHttpMessageHandler<RedditTokenHandler>();

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
app.UseRequestDecompression();
app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapHub<RedditStatsHub>("/stats");
app.Run();
