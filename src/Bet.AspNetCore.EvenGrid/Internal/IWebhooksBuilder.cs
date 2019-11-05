using Bet.AspNetCore.EvenGrid.Models;
using Bet.AspNetCore.EvenGrid.Webhooks;

using Microsoft.Extensions.DependencyInjection;

namespace Bet.AspNetCore.EvenGrid.Internal
{
    /// <summary>
    /// Event Grid WebHook Builder.
    /// </summary>
    public interface IWebhooksBuilder
    {
        IServiceCollection Services { get; }

        /// <summary>
        /// Add Event Grid Webhook.
        /// </summary>
        /// <typeparam name="TWebHook">The event grid webhook processor.</typeparam>
        /// <typeparam name="TEvent">The event grid event type.</typeparam>
        /// <param name="eventTypeName">The name of the event grid grouping.</param>
        /// <returns></returns>
        IWebhooksBuilder AddWebhook<TWebHook, TEvent>(string eventTypeName)
            where TWebHook : class, IWebhook<TEvent>
            where TEvent : WebhookEvent;

        /// <summary>
        /// Adds SignalR hub to be used with the Viewer.
        /// </summary>
        /// <param name="httpRoute">The default values is '/hubs/gridevents'.</param>
        /// <returns></returns>
        IWebhooksBuilder AddViewerHubContext(string httpRoute = "/hubs/gridevents");
    }
}
