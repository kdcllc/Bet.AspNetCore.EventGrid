using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Bet.AspNetCore.EvenGrid.MessageHandlers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Broadcaster.Services
{
    public class EventGridService
    {
        private SasAuthorizeDelegatingHandlerOptions _options;
        private readonly ILogger<EventGridService> _logger;

        public EventGridService(
            IOptionsMonitor<SasAuthorizeDelegatingHandlerOptions> optionsMonitor,
            ILogger<EventGridService> logger)
        {
            _options = optionsMonitor.CurrentValue;
            optionsMonitor.OnChange(newOptions => _options = newOptions);
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        private async Task SendEventWithHttpClient()
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
            _logger.LogInformation($"Sending: {content}");

            var options = new SasAuthorizeDelegatingHandlerOptions(Endpoint, Key, TimeSpan.FromMinutes(8));

            using var client = new HttpClient(new SasAuthorizeDelegatingHandler(options)
            {
                InnerHandler = new HttpClientHandler()
            });

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
                    Data = new EmployeeCreatedEvent
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
    }
}
