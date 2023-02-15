namespace PaulsTwitterFeed
{
    public class TwitterFeed : BackgroundService
    {
        private readonly ILogger<TwitterFeed> logger;

        public TwitterFeed(ILogger<TwitterFeed> logger)
        {
            this.logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation($"${nameof(TwitterFeed)} Background Service Started");
            while (!stoppingToken.IsCancellationRequested)
            {
                await ReadStreamAsync();
            }
            logger.LogInformation($"${nameof(TwitterFeed)} Background Service Stopped");
        }

        private async Task ReadStreamAsync()
        {
            logger.LogInformation("Reading filtered twitter stream");
            await Task.Delay(1000);
        }
    }
}
