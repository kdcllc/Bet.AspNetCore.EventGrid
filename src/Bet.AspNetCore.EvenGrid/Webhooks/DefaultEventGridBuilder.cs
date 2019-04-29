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

        public IEventGridBuilder AddWebhook<TWebhook, TEvent>(string eventTypeName)
           where TEvent : EventGridWebhookEvent
           where TWebhook : class, IEventGridWebhook<TEvent>
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
    }
}
