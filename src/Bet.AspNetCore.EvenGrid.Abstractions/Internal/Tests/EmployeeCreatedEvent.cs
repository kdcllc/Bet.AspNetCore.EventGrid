using Bet.AspNetCore.EvenGrid.Abstractions.Models;

namespace Bet.AspNetCore.EvenGrid.Abstractions.Internal.Tests
{
    internal class EmployeeCreatedEvent : WebhookEvent
    {
        public string Id { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;
    }
}
