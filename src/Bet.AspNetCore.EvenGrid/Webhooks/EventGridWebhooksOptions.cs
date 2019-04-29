using System.Collections.Generic;
using Microsoft.Azure.EventGrid;

namespace Bet.AspNetCore.EvenGrid.Webhooks
{
    public class EventGridWebhooksOptions
    {
        private readonly EventGridSubscriber _eventGridSubscriber;

        public EventGridWebhooksOptions()
        {
            _eventGridSubscriber = new EventGridSubscriber();
        }

        public ICollection<EventGridWebhookRegistration> Registrations { get; } = new List<EventGridWebhookRegistration>();

        public string Route { get; set; }

        public EventGridSubscriber GetEventGridSubscriber()
        {
            foreach (var registration in Registrations)
            {
                _eventGridSubscriber.AddOrUpdateCustomEventMapping(registration.EventGridType, registration.EventType);
            }

            return _eventGridSubscriber;
        }
    }
}
