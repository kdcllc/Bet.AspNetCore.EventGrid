using Bet.AspNetCore.EvenGrid.Webhooks;

namespace Bet.AspNetCore.EvenGrid.Internal
{
    internal class EmployeeCreatedEvent : EventGridWebhookEvent
    {
        public string Id { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;
    }
}
