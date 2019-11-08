using System;
using System.Threading.Tasks;

using Broadcaster.Services;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Broadcaster
{
    // https://stackoverflow.com/questions/50120429/what-is-the-key-to-generate-aeg-sas-token
    internal sealed class Program
    {
        private static IConfiguration _configuration;

        internal static async Task<int> Main(string[] args)
        {
            using var host = CreateHostBuilder(args).UseConsoleLifetime().Build();

            await host.StartAsync();
            var scope = host.Services.CreateScope();
            var eventService = scope.ServiceProvider.GetRequiredService<EventService>();

            await eventService.SendEventWithEventGridClient(3);

            await host.StopAsync();

            return 0;
        }

        internal static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, configBuilder) =>
                {
                        // based on environment Development = dev; Production = prod prefix in Azure Vault.
                        var envName = hostingContext.HostingEnvironment.EnvironmentName;

                        var configuration = configBuilder.AddAzureKeyVault(
                        hostingEnviromentName: envName,
                        usePrefix: false,
                        reloadInterval: TimeSpan.FromSeconds(30));

                        // helpful to see what was retrieved from all of the configuration providers.
                        if (hostingContext.HostingEnvironment.IsDevelopment())
                        {
                            configuration.DebugConfigurations();
                        }

                        _configuration = configuration as IConfiguration;
                })
                .ConfigureLogging((hostingContext, logger) =>
                {
                    logger.AddConfiguration(hostingContext.Configuration);
                    logger.AddConsole();
                    logger.AddDebug();
                })
                .ConfigureServices(services =>
                {
                    var endpoint = _configuration.GetValue<string>("CloudEventClient:Endpoint");
                    var key = _configuration.GetValue<string>("CloudEventClient:Key");
                    services.AddCloudEventClient("BetCloudEventClient", endpoint, key, TimeSpan.FromMinutes(10));

                    services.Configure<EventGridOptions>(options =>
                    {
                        options.Endpoint = endpoint;
                        options.Key = key;
                    });

                    services.AddSingleton<EventService>();
                });
        }
    }
}
