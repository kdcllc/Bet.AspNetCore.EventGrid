using System.Collections.Generic;

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

        public EventGridSubscriber GetEventGridSubscriber()
        {
            foreach (var registration in WebHooksRegistrations)
            {
                _eventGridSubscriber.AddOrUpdateCustomEventMapping(registration.EventGridType, registration.EventType);
            }

            return _eventGridSubscriber;
        }
    }
}
