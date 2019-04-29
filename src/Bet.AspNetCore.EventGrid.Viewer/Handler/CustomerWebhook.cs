using System.Threading;
using System.Threading.Tasks;

using Bet.AspNetCore.EvenGrid.Webhooks;
using Bet.AspNetCore.EventGrid.Viewer.Events;
using Microsoft.Extensions.Logging;

namespace Bet.AspNetCore.EventGrid.Viewer.Handler
{
    public class CustomerWebhook : IEventGridWebhook
    {
        private readonly ILogger<CustomerWebhook> _logger;

        public CustomerWebhook(ILogger<CustomerWebhook> logger)
        {
            _logger = logger;
        }

        public Task<EventGridWebHookResult> ProcessEventAsync(EventGridWebhookEvent @event, CancellationToken cancellationToken = default)
        {
            var data = @event as CustomerCreatedEvent;

            _logger.LogInformation("Executing: {data} ", @event);

            return Task.FromResult<EventGridWebHookResult>(null);
        }
    }
}
