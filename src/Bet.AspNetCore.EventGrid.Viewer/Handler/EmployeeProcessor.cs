﻿using System.Threading;
using System.Threading.Tasks;

using Bet.AspNetCore.EvenGrid.Webhooks;
using Bet.AspNetCore.EventGrid.Viewer.Events;
using Microsoft.Extensions.Logging;

namespace Bet.AspNetCore.EventGrid.Viewer.Handler
{
    public class EmployeeProcessor : IEventGridWebhook
    {
        private readonly ILogger<EmployeeProcessor> _logger;

        public EmployeeProcessor(ILogger<EmployeeProcessor> logger)
        {
            _logger = logger;
        }

        public Task<EventGridWebHookResult> ProcessEventAsync(EventGridWebhookEvent @event, CancellationToken cancellationToken = default)
        {
            var data = @event as EmployeeCreatedEvent;

            _logger.LogInformation("Executing: {data} ", @event);

            return Task.FromResult<EventGridWebHookResult>(null);
        }
    }
}
