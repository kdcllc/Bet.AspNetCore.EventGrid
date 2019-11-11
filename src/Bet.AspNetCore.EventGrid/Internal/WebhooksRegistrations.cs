using System;

namespace Bet.AspNetCore.EventGrid.Internal
{
    internal class WebhooksRegistrations
    {
        private Func<IServiceProvider, object> _factory;

        public WebhooksRegistrations(
            string eventGridType,
            Func<IServiceProvider, object> factory,
            Type eventType,
            Type webhookType)
        {
            EventGridType = eventGridType;
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
            EventType = eventType ?? throw new ArgumentNullException(nameof(eventType));
            WebhookType = webhookType ?? throw new ArgumentNullException(nameof(webhookType));
        }

        public string EventGridType { get; }

        public Type EventType { get; }

        public Type WebhookType { get; }

        public Func<IServiceProvider, object> Factory
        {
            get => _factory;
            set => _factory = value ?? throw new ArgumentNullException(nameof(value));
        }
    }
}
