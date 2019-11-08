using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using Bet.AspNetCore.EventGrid.Abstractions.Models;

namespace Bet.AspNetCore.EventGrid.MessageHandlers
{
    /// <summary>
    /// Azure Event Grid Cloud and Event Grid type of messages.
    /// </summary>
    public interface ICloudEventClient
    {
        /// <summary>
        /// Sends EventGrid CloudEvent type of the event.
        /// </summary>
        /// <typeparam name="T">The type of the event data property of the <see cref="CloudEvent{T}"/>.</typeparam>
        /// <param name="data">The data to be send.</param>
        /// <param name="cancellationToken">A System.Threading.CancellationToken to observe while waiting for a task to complete.</param>
        /// <returns><see cref="Task"/>.</returns>
        Task SendEventAsync<T>(CloudEvent<T> data, CancellationToken cancellationToken = default) where T : class, new();

        /// <summary>
        /// Sends EventGrid EventGrid type of the event.
        /// </summary>
        /// <typeparam name="T">The type of the event data property of the <see cref="GridEvent{T}"/>.</typeparam>
        /// <param name="data"></param>
        /// <param name="cancellationToken">A System.Threading.CancellationToken to observe while waiting for a task to complete.</param>
        /// <returns><see cref="Task"/>.</returns>
        Task SendEventAsync<T>(GridEvent<T> data, CancellationToken cancellationToken = default) where T : class, new();

        /// <summary>
        /// Allows to send content directly.
        /// </summary>
        /// <param name="data">The <see cref="StringContent"/> data.</param>
        /// <param name="cancellationToken">A System.Threading.CancellationToken to observe while waiting for a task to complete.</param>
        /// <returns></returns>
        Task<HttpResponseMessage> SendAsync(StringContent data, CancellationToken cancellationToken = default);
    }
}
