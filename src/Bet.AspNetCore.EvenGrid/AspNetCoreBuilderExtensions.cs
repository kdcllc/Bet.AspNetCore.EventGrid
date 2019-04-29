using Bet.AspNetCore.EvenGrid.Webhooks;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Microsoft.AspNetCore.Builder
{
    public static class AspNetCoreBuilderExtensions
    {
        public static IApplicationBuilder UseEventGridHooks(this IApplicationBuilder app)
        {
            var options = app.ApplicationServices.GetRequiredService<IOptions<EventGridWebhooksOptions>>().Value;

            app.MapWhen(context => context.Request.Path.StartsWithSegments(options.Route), builder =>
            {
                builder.UseMiddleware<EventGridWebhookMiddleware>();
            });
            return app;
        }
    }
}
