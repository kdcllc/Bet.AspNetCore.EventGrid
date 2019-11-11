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
        private static string _type;

        internal static async Task<int> Main(string[] args)
        {
            using var host = CreateHostBuilder(args).UseConsoleLifetime().Build();

            await host.StartAsync();
            var scope = host.Services.CreateScope();
            var eventService = scope.ServiceProvider.GetRequiredService<EventService>();

            if (_type == "cloudevent")
            {
                await eventService.SendCloudEvent();
            }
            else
            {
                await eventService.SendEventWithCloudClient(1);
                await eventService.SendEventWithEventGridClient(1);
            }

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
                    _type = _configuration.GetValue<string>("TopicClientType");

                    var prefix = _type switch
                    {
                        "eventgrid" => "TopicClientEventGrid",
                        "cloudevent" => "TopicClientCloudEvent"
                    };

                    var endpoint = _configuration.GetValue<string>($"{prefix}:Endpoint");
                    var key = _configuration.GetValue<string>($"{prefix}:Key");
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
