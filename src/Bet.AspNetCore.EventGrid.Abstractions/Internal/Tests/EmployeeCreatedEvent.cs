using Bet.AspNetCore.EventGrid.Abstractions.Models;

namespace Bet.AspNetCore.EventGrid.Abstractions.Internal.Tests
{
    internal class EmployeeCreatedEvent : WebhookEvent
    {
        public string Id { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;
    }
}
