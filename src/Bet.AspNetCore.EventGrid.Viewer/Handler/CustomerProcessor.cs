using System.Threading;
using System.Threading.Tasks;

using Bet.AspNetCore.EvenGrid.Webhooks;

using Microsoft.Extensions.Logging;

namespace Bet.AspNetCore.EventGrid.Viewer.Handler
{
    public class CustomerProcessor : IEventGridWebhook
    {
        private readonly ILogger<CustomerProcessor> _logger;

        public CustomerProcessor(ILogger<CustomerProcessor> logger)
        {
            _logger = logger;
        }

        public Task<EventGridWebHookResult> ProcessEventAsync(EventGridWebhookEvent @event, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Executing: {data} ", @event);

            return Task.FromResult<EventGridWebHookResult>(null);
        }
    }
}
