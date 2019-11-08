using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Bet.AspNetCore.EventGrid.Abstractions.Models;

using Newtonsoft.Json;

namespace Bet.AspNetCore.EventGrid.MessageHandlers
{
    /// <inheritdoc />
    public class CloudEventClient : ICloudEventClient
    {
        private readonly HttpClient _httpClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="CloudEventClient"/> class.
        /// </summary>
        /// <param name="httpClient"></param>
        public CloudEventClient(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        /// <inheritdoc />
        public async Task<HttpResponseMessage> SendAsync(StringContent data, CancellationToken cancellationToken = default)
        {
            using var response = await _httpClient.PostAsync(string.Empty, data, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync();
                throw new CloudEventClientException(response.StatusCode, body);
            }

            return response;
        }

        /// <inheritdoc />
        public async Task SendEventAsync<T>(CloudEvent<T> data, CancellationToken cancellationToken = default) where T : class, new()
        {
            var json = JsonConvert.SerializeObject(data);
            await ProcessResponse(json, cancellationToken);
        }

        /// <inheritdoc />
        public async Task SendEventAsync<T>(GridEvent<T> data, CancellationToken cancellationToken = default) where T : class, new()
        {
            var json = JsonConvert.SerializeObject(data);
            await ProcessResponse(json, cancellationToken);
        }

        private async Task ProcessResponse(string json, CancellationToken cancellationToken = default)
        {
            using var content = new StringContent(json, Encoding.UTF8, "application/json");
            using var response = await _httpClient.PostAsync(string.Empty, content, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync();
                throw new CloudEventClientException(response.StatusCode, body);
            }
        }
    }
}
