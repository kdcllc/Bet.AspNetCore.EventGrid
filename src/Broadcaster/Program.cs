using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Bet.AspNetCore.EvenGrid.MessageHanders;
using Bet.AspNetCore.EvenGrid.Models;

using Microsoft.Azure.EventGrid;
using Microsoft.Azure.EventGrid.Models;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Broadcaster
{
#pragma warning disable CA2000 // Dispose objects before losing scope

    // https://stackoverflow.com/questions/50120429/what-is-the-key-to-generate-aeg-sas-token
    internal sealed class Program
    {
        private static readonly string Key = Environment.GetEnvironmentVariable("EVENT_GRID_KEY");
        private static readonly string Endpoint = Environment.GetEnvironmentVariable("EVENT_GRID_URL");

        internal static async Task Main(string[] args)
        {
            // await TaskSendEventWithEventGridClient();
            await SendEventWithHttpClient();
        }

        private static async Task SendEventWithHttpClient()
        {
            var events = new List<object>();

            for (var i = 0; i < 2; i++)
            {
                dynamic payload = new JObject();
                payload.Id = Guid.NewGuid().ToString();
                payload.EventType = "Group.Employee";
                payload.Subject = i;
                payload.EventTime = DateTimeOffset.Now.ToString("o");
                payload.Data = new JObject();
                payload.Data.Id = $"Item #{1}";
                events.Add(payload);
            }

            var content = JsonConvert.SerializeObject(events);
            Console.WriteLine($"Sending: {content}");

            var client = new HttpClient(new SasAuthorizeMessageHandler(new SasAuthorizeOptions(
                Endpoint, Key, TimeSpan.FromMinutes(8)))
            { InnerHandler = new HttpClientHandler() });

            var httpContent = new StringContent(content, Encoding.UTF8, "application/json");
            var result = await client.PostAsync(Endpoint, httpContent);
            var resultText = await result.Content.ReadAsStringAsync();

            Console.WriteLine($"Response: {result.StatusCode} - {resultText}.");
        }

        private static async Task TaskSendEventWithEventGridClient()
        {
            var topicHostName = new Uri(Endpoint).Host;
            var creds = new TopicCredentials(Key);

            var client = new EventGridClient(creds);

            var events = new List<EventGridEvent>();

            for (var i = 0; i < 2; i++)
            {
                events.Add(new EventGridEvent
                {
                    Id = Guid.NewGuid().ToString(),
                    EventType = "Group.Employee",
                    Data = new Employee
                    {
                        Id = $"Item #{1}"
                    },
                    EventTime = DateTime.Now,
                    Subject = $"Subject #{1}",
                    DataVersion = "2.0"
                });

                await client.PublishEventsAsync(topicHostName, events);

                Console.Write("Published events to Event Grid topic.");
            }
        }

        internal class Employee : WebhookEvent
        {
            public string Id { get; set; }
        }
    }
#pragma warning restore CA2000 // Dispose objects before losing scope

}
