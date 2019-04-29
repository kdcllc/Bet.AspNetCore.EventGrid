using System;

using Microsoft.Extensions.DependencyInjection;

namespace Bet.AspNetCore.EvenGrid.Webhooks
{
    public class DefaultEventGridBuilder : IEventGridBuilder
    {
        public IServiceCollection Services { get; }

        public DefaultEventGridBuilder(IServiceCollection services)
        {
            Services = services ?? throw new ArgumentNullException(nameof(services));
        }

        public IEventGridBuilder AddWebhook<TWebhook, TEventType>(string eventTypeName)
           where TEventType : EventGridWebhookEvent
           where TWebhook : class, IEventGridWebhook
        {
            Services.Configure<EventGridWebhooksOptions>(options =>
            {
                options.Registrations.Add(new EventGridWebhookRegistration(
                    eventTypeName,
                    sp => ActivatorUtilities.GetServiceOrCreateInstance<TWebhook>(sp),
                    typeof(TEventType)));
            });

            return this;
        }
    }
}
