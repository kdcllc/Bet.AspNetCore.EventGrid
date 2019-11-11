using System;
using System.Net.Http;

using Bet.AspNetCore.EventGrid.MessageHandlers;
using Bet.AspNetCore.EventGrid.MessageHandlers.Authorization;

using Polly;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class CloudEventClientServiceCollectionExtensions
    {
        /// <summary>
        /// Adds Cloud Event Http Client with <see cref="SasAuthorizeDelegatingHandler"/>.
        /// </summary>
        /// <param name="services">The DI services.</param>
        /// <param name="name">The name of the Http Client which allows creations of many clients.</param>
        /// <param name="endpoint">The Endpoint URL.</param>
        /// <param name="key">The SAS key.</param>
        /// <param name="tokenExpiration">The timespan to expire the SAS token.</param>
        /// <param name="policy">The Polly policies for retries if needed.</param>
        /// <returns></returns>
        public static IServiceCollection AddCloudEventClient(
            this IServiceCollection services,
            string name,
            string endpoint,
            string key,
            TimeSpan? tokenExpiration,
            IAsyncPolicy<HttpResponseMessage>? policy = null)
        {
            services.AddTransient(sp =>
            {
                var options = new SasAuthorizeDelegatingHandlerOptions(endpoint, key, tokenExpiration);
                return new SasAuthorizeDelegatingHandler(options);
            });

            var clientBuilder = services.AddHttpClient<ICloudEventClient, CloudEventClient>(name)
                .ConfigureHttpClient(options =>
                {
                    options.BaseAddress = new Uri(endpoint);
                })
                .AddHttpMessageHandler(sp => sp.GetRequiredService<SasAuthorizeDelegatingHandler>());
            if (policy != null)
            {
                clientBuilder.AddPolicyHandler(policy);
            }

            return services;
        }
    }
}
