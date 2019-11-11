using Bet.AspNetCore.EventGrid.Internal;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

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

            return app;
        }
    }
}
