using System;

namespace Bet.AspNetCore.EvenGrid.Webhooks
{
    public class EventGridWebhookRegistration
    {
        public EventGridWebhookRegistration(
            string eventGridType,
            Func<IServiceProvider, IEventGridWebhook> factory,
            Type eventType)
        {
            EventGridType = eventGridType;
            Factory = factory ?? throw new ArgumentNullException(nameof(factory));
            EventType = eventType ?? throw new ArgumentNullException(nameof(eventType));
        }

        /// <summary>
        /// Event Grid Event type, grouping.
        /// </summary>
        public string EventGridType { get; }

        public Type EventType { get; }

        private Func<IServiceProvider, IEventGridWebhook> _factory;

        /// <summary>
        /// Gets or sets a delegate used to create the <see cref="IEventGridWebhook"/> instance.
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        public Func<IServiceProvider, IEventGridWebhook> Factory
        {
            get => _factory;
            set => _factory = value ?? throw new ArgumentNullException(nameof(value));
        }
    }
}
