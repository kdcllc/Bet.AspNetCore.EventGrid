using System;

using Bet.AspNetCore.EventGrid.Viewer;

using Microsoft.Extensions.DependencyInjection;

namespace Bet.AspNetCore.EvenGrid.Webhooks
{
    internal class DefaultEventGridWebhookBuilder : IEventGridWebhookBuilder
    {
        public DefaultEventGridWebhookBuilder(IServiceCollection services)
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
        public IEventGridWebhookBuilder AddWebhook<TWebhook, TEvent>(string eventTypeName)
           where TWebhook : class, IEventGridWebhook<TEvent>
           where TEvent : EventGridWebhookEvent
        {
            Services.Configure<EventGridWebhooksOptions>(options =>
            {
                options.WebHooksRegistrations.Add(new EventGridWebhookRegistration(
                    eventTypeName,
                    sp => ActivatorUtilities.GetServiceOrCreateInstance<TWebhook>(sp),
                    typeof(TEvent),
                    typeof(TWebhook)));
            });

            return this;
        }

        public IEventGridWebhookBuilder AddViewerHubContext(string httpRoute = "/hubs/gridevents")
        {
            Services.ConfigureOptions<DefaultUIConfigureOptions>();

            Services.Configure<ViewerOptions>(o =>
            {
                o.Route = httpRoute;
            });

            Services.Configure<EventGridWebhooksOptions>(options =>
            {
                options.ViewerHubContextEnabled = true;
                options.ViewerHubContextRoute = httpRoute;
            });

            return this;
        }
    }
}
