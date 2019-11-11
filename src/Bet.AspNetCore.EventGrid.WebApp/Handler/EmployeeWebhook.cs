using System;
using System.Threading;
using System.Threading.Tasks;

using Bet.AspNetCore.EventGrid.Abstractions;
using Bet.AspNetCore.EventGrid.Abstractions.Models;
using Bet.AspNetCore.EventGrid.WebApp.Events;
using Bet.AspNetCore.EventGrid.WebApp.Services;

using Microsoft.Extensions.Logging;

namespace Bet.AspNetCore.EventGrid.WebApp.Handler
{
    public class EmployeeWebhook : IWebhook<EmployeeCreatedEvent>
    {
        private readonly ILogger<EmployeeWebhook> _logger;
        private readonly IOperationService _service;

        public EmployeeWebhook(ILogger<EmployeeWebhook> logger, IOperationService service)
        {
            _logger = logger ?? throw new System.ArgumentNullException(nameof(logger));
            _service = service ?? throw new System.ArgumentNullException(nameof(service));
        }

        public Task<WebHookResult> ProcessEventAsync(EmployeeCreatedEvent @event, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation($"Transient Id: {_service.TransientOperation.OperationId}{Environment.NewLine}" +
                $"Scoped Id: {_service.ScopedOperation.OperationId}{Environment.NewLine}" +
                $"Singleton Id: {_service.SingletonOperation.OperationId}{Environment.NewLine}" +
                $"Singleton Instance Id: {_service.SingletonInstanceOperation.OperationId}{Environment.NewLine}");

            // var data = @event as EmployeeCreatedEvent;
            _logger.LogInformation("Executing: {data} ", @event);

            return Task.FromResult<WebHookResult>(null);
        }
    }
}
