using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using BenchmarkDotNet.Attributes;

using Bet.AspNetCore.EvenGrid.Internal.Tests;
using Bet.AspNetCore.EvenGrid.Models;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

namespace Benchmarks
{
    [InProcess]
    [MemoryDiagnoser]
#pragma warning disable CA1001 // Types that own disposable fields should be disposable
    public class ResponseBenchmarks
#pragma warning restore CA1001 // Types that own disposable fields should be disposable
    {
        private HttpClient _client;
        private TestServer _server;

        [GlobalSetup]
        public void GlobalSetup()
        {
            var builder = new WebHostBuilder()
                                .ConfigureLogging(l =>
                                {
                                    l.ClearProviders();
                                })
                                .ConfigureKestrel(k => k.AllowSynchronousIO = false)
                                .UseStartup<Startup>();

            _server = new TestServer(builder);

            _client = _server.CreateClient();
        }

        [GlobalCleanup]
        public void GlobalCleanup()
        {
            _client?.Dispose();
            _server?.Dispose();
        }

        [Benchmark]
        public async Task<HttpResponseMessage> GetWebHookResponseTime()
        {
            var cloudEvent = new CloudEvent<EmployeeCreatedEvent>(
                new EmployeeCreatedEvent
                {
                    Id = "1234",
                    Name = "Test User"
                },
                "Group.Employee");

            using var request = new HttpRequestMessage(HttpMethod.Post, "/webhooks")
            {
                Content = new StringContent(JsonConvert.SerializeObject(cloudEvent), Encoding.UTF8, "application/json")
            };

            request.Headers.Add("aeg-event-type", "Notification");

            // act
            var response = await _client.SendAsync(request);

            return response;
        }
    }
}
