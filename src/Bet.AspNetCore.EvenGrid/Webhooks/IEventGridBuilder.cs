
using Microsoft.Extensions.DependencyInjection;

namespace Bet.AspNetCore.EvenGrid.Webhooks
{
    public interface IEventGridBuilder
    {
        IServiceCollection Services { get; }

        IEventGridBuilder AddWebhook<TEventProcessor, TEventType>(string eventTypeName)
            where TEventType : EventGridWebhookEvent
            where TEventProcessor : class, IEventGridWebhook ;
    }
}
