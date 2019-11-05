using Bet.AspNetCore.EvenGrid.Internal;
using Bet.AspNetCore.EvenGrid.Models;

using Microsoft.AspNetCore.Http.Connections;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Microsoft.AspNetCore.Builder
{
    public static class AspNetCoreBuilderExtensions
    {
        /// <summary>
        /// Use Event Grid WebHooks middleware.
        /// </summary>
        /// <param name="app">The MVC <see cref="IApplicationBuilder"/>.</param>
        /// <returns></returns>
        public static IApplicationBuilder UseEventGridWebHooks(this IApplicationBuilder app)
        {
            var options = app.ApplicationServices.GetRequiredService<IOptions<WebhooksOptions>>().Value;

            app.MapWhen(context => context.Request.Path.StartsWithSegments(options.HttpRoute), builder =>
            {
                if (options.ViewerHubContextEnabled)
                {
                    app.UseStaticFiles();

                    app.UseWebSockets();
#if NETSTANDARD2_0
                    app.UseSignalR((configure) =>
                    {
                        var desiredTransports =
                            HttpTransportType.WebSockets |
                            HttpTransportType.LongPolling;

                        configure.MapHub<GridEventsHub>(options.ViewerHubContextRoute, (opt) =>
                        {
                            opt.Transports = desiredTransports;
                        });
                    });
#endif
                }

                builder.UseMiddleware<WebhookMiddleware>();
            });

            return app;
        }
    }
}
