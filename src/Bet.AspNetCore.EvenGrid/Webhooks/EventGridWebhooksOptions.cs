﻿using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.AspNetCore.Http;
using Microsoft.Azure.EventGrid;

namespace Bet.AspNetCore.EvenGrid.Webhooks
{
    internal class EventGridWebhooksOptions
    {
        private readonly EventGridSubscriber _eventGridSubscriber;

        public EventGridWebhooksOptions()
        {
            _eventGridSubscriber = new EventGridSubscriber();
        }

        public ICollection<EventGridWebhookRegistration> WebHooksRegistrations { get; } = new List<EventGridWebhookRegistration>();

        public string HttpRoute { get; set; }

        public string HttpMethod { get; set; } = "POST";

        public bool ThrowIfException { get; set; } = true;

        public bool ViewerHubContextEnabled { get; set; } = false;

        public string ViewerHubContextRoute { get; set; }

        public EventGridSubscriber GetEventGridSubscriber()
        {
            foreach (var registration in WebHooksRegistrations)
            {
                _eventGridSubscriber.AddOrUpdateCustomEventMapping(registration.EventGridType, registration.EventType);
            }

            return _eventGridSubscriber;
        }

        public Func<HttpContext, bool> EventTypeSubcriptionValidation = (context) => context.Request.Headers["aeg-event-type"].FirstOrDefault() == "SubscriptionValidation";

        public Func<HttpContext, bool> EventTypeNotification = (context) => context.Request.Headers["aeg-event-type"].FirstOrDefault() == "Notification";
    }
}
