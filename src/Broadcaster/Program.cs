using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Bet.AspNetCore.EvenGrid.Webhooks;

using Microsoft.Azure.EventGrid;
using Microsoft.Azure.EventGrid.Models;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Broadcaster
{
    //https://stackoverflow.com/questions/50120429/what-is-the-key-to-generate-aeg-sas-token
    class Program
    {
        private static readonly string Key = Environment.GetEnvironmentVariable("EVENT_GRID_KEY");
        private static readonly string Endpoint = Environment.GetEnvironmentVariable("EVENT_GRID_URL");

        static  async Task Main(string[] args)
        {
            await TaskSendEventWithEventGridClient();

            // await SendEventWithHttpClient();
        }

        private static async Task SendEventWithHttpClient()
        {
            var events = new List<object>();

            for (var i = 0; i < 2; i++)
            {
                dynamic payload = new JObject();
                payload.Id = Guid.NewGuid().ToString();
                payload.EventType = "Event.Group.One";
                payload.Subject = i;
                payload.EventTime = DateTimeOffset.Now.ToString("o");
                payload.Data = new JObject();
                payload.Data.Id = $"Item #{1}";
                events.Add(payload);
            }

            var content = JsonConvert.SerializeObject(events);
            Console.WriteLine($"Sending: {content}");

            var client = new HttpClient();
            // client.DefaultRequestHeaders.Add("aeg-sas-key", Key); or token

            var sasToken = BuildSharedAccessSignature($"{Endpoint}?api-version=2018-01-01", DateTime.UtcNow + TimeSpan.FromMinutes(8), Key);

            client.DefaultRequestHeaders.Add("aeg-sas-token", sasToken);

            var httpContent = new StringContent(content, Encoding.UTF8, "application/json");
            var result = await client.PostAsync(Endpoint, httpContent);
            var resultText = await result.Content.ReadAsStringAsync();

            // https://docs.microsoft.com/en-us/azure/event-grid/security-authentication#sas-tokens
            static string BuildSharedAccessSignature(string resource, DateTime expirationUtc, string key)
            {
                const char Resource = 'r';
                const char Expiration = 'e';
                const char Signature = 's';

                var encodedResource = HttpUtility.UrlEncode(resource);
                var culture = CultureInfo.CreateSpecificCulture("en-US");
                var encodedExpirationUtc = HttpUtility.UrlEncode(expirationUtc.ToString(culture));

                var unsignedSas = $"{Resource}={encodedResource}&{Expiration}={encodedExpirationUtc}";
                using (var hmac = new HMACSHA256(Convert.FromBase64String(key)))
                {
                    var signature = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(unsignedSas)));
                    string encodedSignature = HttpUtility.UrlEncode(signature);
                    var signedSas = $"{unsignedSas}&{Signature}={encodedSignature}";

                    return signedSas;
                }
            }

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
                    EventType = "Event.Group.One",
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

        internal class Employee : EventGridWebhookEvent
        {
            public string Id { get; set; }
        }
    }
}
