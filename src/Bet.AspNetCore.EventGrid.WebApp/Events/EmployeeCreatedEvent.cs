using Bet.AspNetCore.EvenGrid.Abstractions.Models;

namespace Bet.AspNetCore.EventGrid.WebApp.Events
{
    public class EmployeeCreatedEvent : WebhookEvent
    {
        public string Id { get; set; }

        public string Name { get; set; }
    }
}
