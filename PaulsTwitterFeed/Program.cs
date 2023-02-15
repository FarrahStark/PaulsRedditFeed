using PaulsTwitterFeed;

var builder = WebApplication.CreateBuilder(args);
var settings = builder.Configuration.GetSection("AppSettings").Get<AppSettings>();
if (settings?.TwitterApiKey == null)
{
    var message = "Unable to find AppSettings section from app configuration. Check appsettings.json and secrets configuration.";
    throw new SettingsLoadException(message);
}

// Add services to the container.
builder.Services.AddLogging(loggers => loggers.AddConsole());
builder.Services.AddControllersWithViews();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddHttpClient<TwitterFeed>()
    .ConfigureHttpClient(httpClient =>
    {
        httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer ${settings.TwitterApiKey}");
        httpClient.BaseAddress = new Uri("https://api.twitter.com/2/search/");
    });
builder.Services.AddHostedService<TwitterFeed>();

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

app.MapFallbackToFile("index.html"); ;

app.Run();
