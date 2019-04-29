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

        public IEventGridBuilder AddEventProcessor<TEventProcessor, TEventType>(string eventTypeName)
           where TEventType : EventGridWebhookEvent
           where TEventProcessor : class, IEventGridWebhook
        {
            Services.Configure<EventGridWebhooksOptions>(options =>
            {
                options.Registrations.Add(new EventGridWebhookRegistration(
                    eventTypeName,
                    sp => ActivatorUtilities.GetServiceOrCreateInstance<TEventProcessor>(sp),
                    typeof(TEventType)));
            });

            return this;
        }
    }
}
