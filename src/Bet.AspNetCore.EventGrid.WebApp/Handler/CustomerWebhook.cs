using System.Threading;
using System.Threading.Tasks;

using Bet.AspNetCore.EventGrid.Abstractions;
using Bet.AspNetCore.EventGrid.Abstractions.Models;
using Bet.AspNetCore.EventGrid.Diagnostics;
using Bet.AspNetCore.EventGrid.WebApp.Events;

using Microsoft.Extensions.Logging;

namespace Bet.AspNetCore.EventGrid.WebApp.Handler
{
    public class CustomerWebhook : IWebhook<CustomerCreatedEvent>
    {
        private readonly ILogger<CustomerWebhook> _logger;
        private readonly IProcessReport _processReport;

        public CustomerWebhook(ILogger<CustomerWebhook> logger, IProcessReport processReport)
        {
            _logger = logger;
            _processReport = processReport;
        }

        public Task<WebHookResult> ProcessEventAsync(CustomerCreatedEvent @event, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Executing: {data} ", @event);

            _processReport.IncrementSuccess();

            return Task.FromResult<WebHookResult>(null);
        }
    }
}
