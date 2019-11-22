using System.Linq;

using Bet.AspNetCore.EventGrid.Diagnostics;
using Bet.AspNetCore.EventGrid.Internal;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Newtonsoft.Json;

namespace Microsoft.AspNetCore.Builder
{
    public static class WebhooksAspNetCoreBuilderExtensions
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
                    app.UseEventGridWebHooksViewer(options.ViewerHubContextRoute);
                }

                builder.UseMiddleware<WebhookMiddleware>();
            });

            if (!string.IsNullOrEmpty(options.Diagnostics))
            {
                app.Map(new PathString(options.Diagnostics), configure =>
                {
                    configure.Run(async context =>
                    {
                        var service = configure.ApplicationServices.GetRequiredService<IProcessReport>();
                        var reset = context.Request.Query["reset"];

                        if (reset.Contains("true"))
                        {
                            service.Reset();
                        }

                        var content = new
                        {
                            Succeed = service.SuccessCount.ToString(),
                            Failed = service.FailureCount.ToString()
                        };

                        var json = JsonConvert.SerializeObject(content);
                        await context.Response.WriteAsync(json);
                    });
                });
            }

            return app;
        }
    }
}
