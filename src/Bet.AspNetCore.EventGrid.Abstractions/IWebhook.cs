using System.Threading;
using System.Threading.Tasks;

using Bet.AspNetCore.EventGrid.Abstractions.Models;

namespace Bet.AspNetCore.EventGrid.Abstractions
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
