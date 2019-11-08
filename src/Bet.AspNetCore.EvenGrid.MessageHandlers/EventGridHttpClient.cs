using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Bet.AspNetCore.EvenGrid.Abstractions.Models;

using Newtonsoft.Json;

namespace Bet.AspNetCore.EvenGrid.MessageHandlers
{
    /// <inheritdoc />
    public class EventGridHttpClient : IEventGridHttpClient
    {
        private readonly HttpClient _httpClient;

        public EventGridHttpClient(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public async Task SendEventAsync<T>(CloudEvent<T> data, CancellationToken cancellationToken = default) where T : class, new()
        {
            var json = JsonConvert.SerializeObject(data);
            await ProcessResponse(json, cancellationToken);
        }

        public async Task SendEventAsync<T>(GridEvent<T> data, CancellationToken cancellationToken = default) where T : class, new()
        {
            var json = JsonConvert.SerializeObject(data);
            await ProcessResponse(json, cancellationToken);
        }

        private async Task ProcessResponse(string json, CancellationToken cancellationToken = default)
        {
            using (var content = new StringContent(json, Encoding.UTF8, "application/json"))
            {
                using (var response = await _httpClient.PostAsync(string.Empty, content, cancellationToken))
                {
                    if (!response.IsSuccessStatusCode)
                    {
                        var body = await response.Content.ReadAsStringAsync();
                        throw new EventGridHttpClientException(response.StatusCode, body);
                    }
                }
            }
        }
    }
}
