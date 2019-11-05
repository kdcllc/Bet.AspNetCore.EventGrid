using System;

using Bet.AspNetCore.EvenGrid.Models;
using Bet.AspNetCore.EvenGrid.Webhooks;
using Bet.AspNetCore.EventGrid.Viewer;

using Microsoft.Extensions.DependencyInjection;

namespace Bet.AspNetCore.EvenGrid.Internal
{
    internal class WebhookBuilder : IWebhooksBuilder
    {
        public WebhookBuilder(IServiceCollection services)
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
           where TWebhook : class, IEventGridWebhook<TEvent>
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

        public IWebhooksBuilder AddViewerHubContext(string httpRoute = "/hubs/gridevents")
        {
            Services.ConfigureOptions<DefaultUIConfigureOptions>();

            Services.Configure<ViewerOptions>(o =>
            {
                o.Route = httpRoute;
            });

            Services.Configure<WebhooksOptions>(options =>
            {
                options.ViewerHubContextEnabled = true;
                options.ViewerHubContextRoute = httpRoute;
            });

            return this;
        }
    }
}
