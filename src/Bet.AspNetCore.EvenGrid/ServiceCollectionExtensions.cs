using Bet.AspNetCore.EvenGrid.Webhooks;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IEventGridBuilder AddEvenGridWebhooks(
            this IServiceCollection services,
            string route = "/webhooks")
        {
            var builder = new DefaultEventGridBuilder(services);

            builder.Services.Configure<EventGridWebhooksOptions>(options =>
            {
                options.HttpRoute = route;
            });

            return builder;
        }
    }
}
