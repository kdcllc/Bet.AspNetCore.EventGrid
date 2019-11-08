using System;

using Bet.AspNetCore.EvenGrid.Abstractions;
using Bet.AspNetCore.EvenGrid.Abstractions.Models;

using Microsoft.Extensions.DependencyInjection;

namespace Bet.AspNetCore.EvenGrid.Internal
{
    internal class WebhooksBuilder : IWebhooksBuilder
    {
        public WebhooksBuilder(IServiceCollection services)
        {
            Services = services ?? throw new ArgumentNullException(nameof(services));
        }

        public IServiceCollection Services { get; }

        /// <summary>
        /// Add Webhook to the collection of the webhooks.
        /// </summary>
        /// <typeparam name="TWebhook">The type of event grid webhook processor.</typeparam>
        /// <typeparam name="TEvent">The type of the event grid webhook event.</typeparam>
        /// <param name="eventTypeName">The name of the event grid grouping.</param>
        /// <returns></returns>
        public IWebhooksBuilder AddWebhook<TWebhook, TEvent>(string eventTypeName)
           where TWebhook : class, IWebhook<TEvent>
           where TEvent : WebhookEvent
        {
            Services.Configure<WebhooksOptions>(options =>
            {
                options.WebHooksRegistrations.Add(new WebhooksRegistrations(
                    eventTypeName,
                    sp => ActivatorUtilities.GetServiceOrCreateInstance<TWebhook>(sp),
                    typeof(TEvent),
                    typeof(TWebhook)));
            });

            return this;
        }

        public IWebhooksBuilder AddViewerSignalRHubContext(string httpRoute = "/hubs/gridevents")
        {
            Services.AddViewerSignalR(httpRoute);

            Services.Configure<WebhooksOptions>(options =>
            {
                options.ViewerHubContextEnabled = true;
                options.ViewerHubContextRoute = httpRoute;
            });

            return this;
        }
    }
}
