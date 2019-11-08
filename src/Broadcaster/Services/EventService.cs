using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Bet.AspNetCore.EventGrid.Abstractions.Internal.Tests;
using Bet.AspNetCore.EventGrid.MessageHandlers;
using Bet.AspNetCore.EventGrid.MessageHandlers.Authorization;
using Microsoft.Azure.EventGrid;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Broadcaster.Services
{
    public class EventService
    {
        private readonly ILogger<EventService> _logger;
        private readonly ICloudEventClient _cloudEventClient;
        private readonly IHostApplicationLifetime _applicationLifetime;
        private readonly EventGridOptions _options;

        public EventService(
            ICloudEventClient cloudEventClient,
            IHostApplicationLifetime applicationLifetime,
            IOptions<EventGridOptions> options,
            ILogger<EventService> logger)
        {
            _cloudEventClient = cloudEventClient ?? throw new ArgumentNullException(nameof(cloudEventClient));
            _applicationLifetime = applicationLifetime ?? throw new ArgumentNullException(nameof(applicationLifetime));
            _options = options.Value;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task SendDynamicRequest(int count = 2)
        {
            var events = new List<object>();

            for (var i = 0; i < count; i++)
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

            using var httpContent = new StringContent(content, Encoding.UTF8, "application/json");
            var result = await _cloudEventClient.SendAsync(httpContent, _applicationLifetime.ApplicationStopping);
            var resultText = await result.Content.ReadAsStringAsync();
            _logger.LogInformation($"Response: {result.StatusCode} - {resultText}.");
        }

        public async Task SendEventWithEventGridClient(int count = 2)
        {
            var topicHostName = new Uri(_options.Endpoint).Host;
            var creds = new TopicCredentials(_options.Key);

            using var client = new EventGridClient(creds);

            var events = new List<EventGridEvent>();

            for (var i = 0; i < count; i++)
            {
                events.Add(new EventGridEvent
                {
                    Id = Guid.NewGuid().ToString(),
                    EventType = "Group.Employee",
                    Data = new EmployeeCreatedEvent
                    {
                        Id = $"Item #{i}",
                        Name = $"Name #{i}"
                    },
                    EventTime = DateTime.Now,
                    Subject = $"Subject #{i}",
                    DataVersion = "2.0"
                });

                await client.PublishEventsAsync(topicHostName, events);

                _logger.LogInformation("Published events to Event Grid topic.");
            }
        }
    }
}
