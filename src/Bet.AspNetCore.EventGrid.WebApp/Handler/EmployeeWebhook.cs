using System.Threading;
using System.Threading.Tasks;

using Bet.AspNetCore.EvenGrid.Webhooks;
using Bet.AspNetCore.EventGrid.WebApp.Events;
using Microsoft.Extensions.Logging;

namespace Bet.AspNetCore.EventGrid.WebApp.Handler
{
    public class EmployeeWebhook : IEventGridWebhook<EmployeeCreatedEvent>
    {
        private readonly ILogger<EmployeeWebhook> _logger;

        public EmployeeWebhook(ILogger<EmployeeWebhook> logger)
        {
            _logger = logger;
        }

        public Task<EventGridWebHookResult> ProcessEventAsync(EmployeeCreatedEvent @event, CancellationToken cancellationToken = default)
        {
            // var data = @event as EmployeeCreatedEvent;

            _logger.LogInformation("Executing: {data} ", @event);

            return Task.FromResult<EventGridWebHookResult>(null);
        }
    }
}
