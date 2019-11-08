﻿using Bet.AspNetCore.EventGrid.Internal;
using Bet.AspNetCore.EventGrid.SasKey;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class WebhooksServiceCollectionExtensions
    {
        /// <summary>
        /// Adds Event Grid Webhooks middleware configurations.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <param name="route">The Http route for the webhooks to be processed. The default value is '/webhooks'.</param>
        /// <param name="method">The HTTP method. The default value is 'POST'.</param>
        /// <param name="throwIfException">Throw exceptions within the middleware. The default is 'true'.</param>
        /// <returns></returns>
        public static IWebhooksBuilder AddEventGridWebhooks(
            this IServiceCollection services,
            string route = "/webhooks",
            string method = "POST",
            bool throwIfException = true)
        {
            var builder = new WebhooksBuilder(services);

            builder.Services.AddTransient<WebhookMiddleware>();

            builder.Services.Configure<WebhooksOptions>(options =>
            {
                options.HttpRoute = route;
                options.HttpMethod = method;
                options.ThrowIfException = throwIfException;
            });

            return builder;
        }

        public static IServiceCollection AddSasKeyValidation(this IServiceCollection services)
        {
            services.AddScoped<SasKeyValidator>();
            return services;
        }
    }
}
