using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace PaulsRedditFeed.Tests;

/// <summary>
/// A factory to create an in memory version of PaulsRedditFeed to run integration tests against
/// </summary>
/// <inheritdoc cref="WebApplicationFactory{TEntryPoint}"/>
internal class PaulsRedditFeedWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly Action<IServiceCollection> configureServices;

    internal PaulsRedditFeedWebApplicationFactory(Action<IServiceCollection>? configureServices = null)
    {
        this.configureServices = configureServices ?? new Action<IServiceCollection>(s => { });
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="builder"></param>
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Alter the registerd services in TProgram.cs and replace them with
            // mocked services as needed. Services mocked here are mocked globally for all tests
            // override the 
            #region example

            // Example Below:
            // Replaces the EF Core DbContext with a memory data store for testing:
            /*
            var dbContextDescriptor = services.SingleOrDefault(
                d => d.ServiceType ==
                    typeof(DbContextOptions<ApplicationDbContext>));

            services.Remove(dbContextDescriptor);

            var dbConnectionDescriptor = services.SingleOrDefault(
                d => d.ServiceType ==
                    typeof(DbConnection));

            services.Remove(dbConnectionDescriptor);

            // Create open SqliteConnection so EF won't automatically close it.
            services.AddSingleton<DbConnection>(container =>
            {
                var connection = new SqliteConnection("DataSource=:memory:");
                connection.Open();

                return connection;
            });

            services.AddDbContext<ApplicationDbContext>((container, options) =>
            {
                var connection = container.GetRequiredService<DbConnection>();
                options.UseSqlite(connection);
            });
            */
            #endregion example
            configureServices(services);
        });

        builder.UseEnvironment("Testing");
    }
}