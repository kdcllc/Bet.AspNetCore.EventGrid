using Bet.AspNetCore.EvenGrid.Abstractions.Models;

namespace Bet.AspNetCore.EventGrid.WebApp.Events
{
    public class CustomerCreatedEvent : WebhookEvent
    {
        public string Id { get; set; }

        public string FullName { get; set; }
    }
}
