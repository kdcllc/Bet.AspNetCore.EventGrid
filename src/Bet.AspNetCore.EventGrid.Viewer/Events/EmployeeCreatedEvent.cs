using Bet.AspNetCore.EvenGrid.Webhooks;

namespace Bet.AspNetCore.EventGrid.Viewer.Events
{
    public class EmployeeCreatedEvent : EventGridWebhookEvent
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
}
