using System.Threading;
using System.Threading.Tasks;

using Bet.AspNetCore.EvenGrid.Models;

namespace Bet.AspNetCore.EvenGrid.Webhooks
{
    /// <summary>
    /// An event grid webhook processor interface.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IWebhook<T>
    {
        /// <summary>
        /// Processes an event grid event.
        /// </summary>
        /// <param name="webHookEvent"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<WebHookResult> ProcessEventAsync(T webHookEvent, CancellationToken cancellationToken = default);
    }
}
