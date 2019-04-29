using System;

namespace Bet.AspNetCore.EvenGrid.Webhooks
{
    public class EventGridWebhookRegistration
    {
        public EventGridWebhookRegistration(
            string eventGridType,
            Func<IServiceProvider, object> factory,
            Type eventType,
            Type webhookType)
        {
            EventGridType = eventGridType;
            Factory = factory ?? throw new ArgumentNullException(nameof(factory));
            EventType = eventType ?? throw new ArgumentNullException(nameof(eventType));
            WebhookType = webhookType ?? throw new ArgumentNullException(nameof(webhookType));

        }

        /// <summary>
        /// Event Grid Event type, grouping.
        /// </summary>
        public string EventGridType { get; }

        public Type EventType { get; }

        public Type WebhookType { get; }

        private Func<IServiceProvider, object> _factory;

        /// <summary>
        /// Gets or sets a delegate used to create the <see cref="IEventGridWebhook"/> instance.
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        public Func<IServiceProvider, object> Factory
        {
            get => _factory;
            set => _factory = value ?? throw new ArgumentNullException(nameof(value));
        }
    }
}
