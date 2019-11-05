using System;
using System.Threading;
using System.Threading.Tasks;

using Bet.AspNetCore.EvenGrid.Models;
using Bet.AspNetCore.EvenGrid.Webhooks;

using Microsoft.Extensions.Logging;

namespace Bet.AspNetCore.EvenGrid.Internal
{
    internal class EmployeeWebhook : IEventGridWebhook<EmployeeCreatedEvent>
    {
        private readonly ILogger<EmployeeWebhook> _logger;

        public EmployeeWebhook(ILogger<EmployeeWebhook> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public Task<WebHookResult> ProcessEventAsync(
            EmployeeCreatedEvent webHookEvent,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Executing: {data} ", webHookEvent);

            return Task.FromResult(new WebHookResult());
        }
    }
}
