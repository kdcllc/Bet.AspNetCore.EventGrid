using System.Threading;
using System.Threading.Tasks;

namespace Bet.AspNetCore.EvenGrid.Webhooks
{
    /// <summary>
    /// An event grid webhook processor interface.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IEventGridWebhook<T>
    {
        /// <summary>
        /// Processes an event grid event.
        /// </summary>
        /// <param name="event"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<EventGridWebHookResult> ProcessEventAsync(T @event, CancellationToken cancellationToken = default);
    }
}
