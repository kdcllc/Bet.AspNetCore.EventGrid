﻿using Bet.AspNetCore.EventGrid.Abstractions;
using Bet.AspNetCore.EventGrid.Abstractions.Models;

namespace Microsoft.Extensions.DependencyInjection
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
        IWebhooksBuilder AddViewerSignalRHubContext(string httpRoute = "/hubs/gridevents");

        /// <summary>
        /// Adds Diagnostics.
        /// </summary>
        /// <param name="diagnosticsPath"></param>
        /// <returns></returns>
        IWebhooksBuilder AddDiagnostics(string diagnosticsPath = "/check");
    }
}
