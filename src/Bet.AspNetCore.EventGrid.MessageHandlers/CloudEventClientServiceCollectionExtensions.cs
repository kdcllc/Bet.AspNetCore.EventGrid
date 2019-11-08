using System;
using System.Net.Http;

using Bet.AspNetCore.EventGrid.MessageHandlers;
using Bet.AspNetCore.EventGrid.MessageHandlers.Authorization;

using Polly;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class CloudEventClientServiceCollectionExtensions
    {
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
                                        .AddHttpMessageHandler(sp => sp.GetRequiredService<SasAuthorizeDelegatingHandler>());
            if (policy != null)
            {
                clientBuilder.AddPolicyHandler(policy);
            }

            return services;
        }
    }
}
