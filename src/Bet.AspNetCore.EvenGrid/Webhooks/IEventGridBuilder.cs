
using Microsoft.Extensions.DependencyInjection;

namespace Bet.AspNetCore.EvenGrid.Webhooks
{
    public interface IEventGridBuilder
    {
        IServiceCollection Services { get; }

        IEventGridBuilder AddWebhook<TWebHook, TEvent>(string eventTypeName)
            where TEvent : EventGridWebhookEvent
            where TWebHook : class, IEventGridWebhook<TEvent>;
    }
}
