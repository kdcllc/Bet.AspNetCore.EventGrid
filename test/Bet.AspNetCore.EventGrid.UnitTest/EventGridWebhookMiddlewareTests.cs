using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Bet.AspNetCore.EvenGrid.Internal.Tests;
using Bet.AspNetCore.EvenGrid.Models;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

using Xunit;

namespace Bet.AspNetCore.EventGrid.UnitTest
{
    public class EventGridWebhookMiddlewareTests
    {
        [Fact]
        public async Task Should_Submit_WebhookEvent_Fails_Not_Found()
        {
            // assign
            var gridEvent = new GridEvent<EmployeeCreatedEvent>(new EmployeeCreatedEvent());
            const string url = "/webhooks";

            var builder = new WebHostBuilder()
                          .ConfigureLogging(logBuilder => logBuilder.AddConsole())
                          .ConfigureKestrel(ko => ko.AllowSynchronousIO = false)
                          .UseStartup<Startup>();

            using var server = new TestServer(builder);
            using var client = server.CreateClient();
            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new StringContent(JsonConvert.SerializeObject(gridEvent), Encoding.UTF8, "application/json")
            };

            request.Headers.Add("aeg-event-type", "Notification");

            // act
            var exMessage = "WebhookMiddleware raised exceptions. (Can't find a Event Grid Webhook for this grid event: EmployeeCreatedEvent)";
            var ex = await Assert.ThrowsAsync<AggregateException>(async () => await client.SendAsync(request));

            Assert.Equal(exMessage, ex.Message);
        }

        [Fact]
        public async Task Should_Submit_WebhookEvent_Success()
        {
            // assign
            var gridEvent = new GridEvent<EmployeeCreatedEvent>(new EmployeeCreatedEvent(), "Group.Employee");
            const string url = "/webhooks";

            var builder = new WebHostBuilder()
                          .ConfigureLogging(logBuilder => logBuilder.AddConsole())
                          .ConfigureKestrel(ko => ko.AllowSynchronousIO = false)
                          .UseStartup<Startup>();

            using var server = new TestServer(builder);
            using var client = server.CreateClient();
            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new StringContent(JsonConvert.SerializeObject(gridEvent), Encoding.UTF8, "application/json")
            };

            request.Headers.Add("aeg-event-type", "Notification");

            // act
            var response = await client.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
