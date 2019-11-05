using Bet.AspNetCore.EvenGrid.Models;

namespace Bet.AspNetCore.EvenGrid.Internal.Tests
{
    internal class EmployeeCreatedEvent : WebhookEvent
    {
        public string Id { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;
    }
}
