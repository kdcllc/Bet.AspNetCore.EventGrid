using System.Threading;
using System.Threading.Tasks;

namespace Bet.AspNetCore.EvenGrid.Webhooks
{
    public interface IEventGridWebhook
    {
        Task<EventGridWebHookResult> ProcessEventAsync(
            EventGridWebhookEvent @event,
            CancellationToken cancellationToken = default);
    }
}
