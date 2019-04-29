using System.Threading;
using System.Threading.Tasks;

namespace Bet.AspNetCore.EvenGrid.Webhooks
{
    public interface IEventGridWebhook<T>
    {
        Task<EventGridWebHookResult> ProcessEventAsync(
            T @event,
            CancellationToken cancellationToken = default);
    }
}
