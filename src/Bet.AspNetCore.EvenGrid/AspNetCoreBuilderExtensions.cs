using Bet.AspNetCore.EvenGrid.Webhooks;

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
            var options = app.ApplicationServices.GetRequiredService<IOptions<EventGridWebhooksOptions>>().Value;

            app.MapWhen(context => context.Request.Path.StartsWithSegments(options.HttpRoute), builder =>
            {
                builder.UseMiddleware<EventGridWebhookMiddleware>();
            });
            return app;
        }
    }
}
