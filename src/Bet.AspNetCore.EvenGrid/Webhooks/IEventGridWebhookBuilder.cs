
using Microsoft.Extensions.DependencyInjection;

namespace Bet.AspNetCore.EvenGrid.Webhooks
{
    /// <summary>
    /// Event Grid WebHook Builder.
    /// </summary>
    public interface IEventGridBuilder
    {
        IServiceCollection Services { get; }

        IEventGridBuilder AddWebhook<TWebHook, TEvent>(string eventTypeName)
            where TEvent : EventGridWebhookEvent
            where TWebHook : class, IEventGridWebhook<TEvent>;
    }
}
