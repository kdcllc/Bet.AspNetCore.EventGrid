using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.AspNetCore.Http;

namespace Bet.AspNetCore.EventGrid.Internal
{
    internal class WebhooksOptions
    {
        public Func<HttpContext, bool> EventTypeSubscriptionValidation { get; } = (context) => context.Request.Headers["aeg-event-type"].FirstOrDefault() == "SubscriptionValidation";

        public Func<HttpContext, bool> EventTypeNotification { get; } = (context) => context.Request.Headers["aeg-event-type"].FirstOrDefault() == "Notification";

        public ICollection<WebhooksRegistrations> WebHooksRegistrations { get; } = new List<WebhooksRegistrations>();

        public string HttpRoute { get; set; } = string.Empty;

        public string HttpMethod { get; set; } = "POST";

        public bool ThrowIfException { get; set; } = true;

        public bool ViewerHubContextEnabled { get; set; } = false;

        public string ViewerHubContextRoute { get; set; } = string.Empty;

        public string Diagnostics { get; set; } = string.Empty;
    }
}
