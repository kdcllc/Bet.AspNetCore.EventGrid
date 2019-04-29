using Bet.AspNetCore.EvenGrid.Webhooks;

namespace Bet.AspNetCore.EventGrid.WebApp.Events
{
    public class EmployeeCreatedEvent : EventGridWebhookEvent
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
}
