using System;

using Bet.AspNetCore.EventGrid.Abstractions;
using Bet.AspNetCore.EventGrid.Abstractions.Models;
using Bet.AspNetCore.EventGrid.Diagnostics;
using Microsoft.Extensions.DependencyInjection;

namespace Bet.AspNetCore.EventGrid.Internal
{
    internal class WebhooksBuilder : IWebhooksBuilder
    {
        public WebhooksBuilder(IServiceCollection services)
        {
            Services = services ?? throw new ArgumentNullException(nameof(services));
        }

        public IServiceCollection Services { get; }

        /// <summary>
        /// Add Webhook to the collection of the webhooks.
        /// </summary>
        /// <typeparam name="TWebhook">The type of event grid webhook processor.</typeparam>
        /// <typeparam name="TEvent">The type of the event grid webhook event.</typeparam>
        /// <param name="eventTypeName">The name of the event grid grouping.</param>
        /// <returns></returns>
        public IWebhooksBuilder AddWebhook<TWebhook, TEvent>(string eventTypeName)
           where TWebhook : class, IWebhook<TEvent>
           where TEvent : WebhookEvent
        {
            Services.Configure<WebhooksOptions>(options =>
            {
                options.WebHooksRegistrations.Add(new WebhooksRegistrations(
                    eventTypeName,
                    sp => ActivatorUtilities.GetServiceOrCreateInstance<TWebhook>(sp),
                    typeof(TEvent),
                    typeof(TWebhook)));
            });

            return this;
        }

        /// <summary>
        /// Adds SignalR Viewer.
        /// </summary>
        /// <param name="httpRoute"></param>
        /// <returns></returns>
        public IWebhooksBuilder AddViewerSignalRHubContext(string httpRoute = "/hubs/gridevents")
        {
            Services.AddViewerSignalR(httpRoute);

            Services.Configure<WebhooksOptions>(options =>
            {
                options.ViewerHubContextEnabled = true;
                options.ViewerHubContextRoute = httpRoute;
            });

            return this;
        }

        /// <summary>
        /// Adds Diagnostics.
        /// </summary>
        /// <param name="diagnosticsPath"></param>
        /// <returns></returns>
        public IWebhooksBuilder AddDiagnostics(string diagnosticsPath = "/check")
        {
            Services.Configure<WebhooksOptions>(options =>
            {
                options.Diagnostics = diagnosticsPath;
            });

            Services.AddSingleton<IProcessReport, ProcessReport>();

            return this;
        }
    }
}
