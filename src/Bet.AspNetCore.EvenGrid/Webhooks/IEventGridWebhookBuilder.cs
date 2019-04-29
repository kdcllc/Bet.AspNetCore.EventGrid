
using Microsoft.Extensions.DependencyInjection;

namespace Bet.AspNetCore.EvenGrid.Webhooks
{
    /// <summary>
    /// Event Grid WebHook Builder.
    /// </summary>
    public interface IEventGridWebhookBuilder
    {
        IServiceCollection Services { get; }

        /// <summary>
        /// Add Event Grid Webhook.
        /// </summary>
        /// <typeparam name="TWebHook">The event grid webhook processor.</typeparam>
        /// <typeparam name="TEvent">The event grid event type.</typeparam>
        /// <param name="eventTypeName">The name of the event grid grouping.</param>
        /// <returns></returns>
        IEventGridWebhookBuilder AddWebhook<TWebHook, TEvent>(string eventTypeName)
            where TEvent : EventGridWebhookEvent
            where TWebHook : class, IEventGridWebhook<TEvent>;
    }
}
