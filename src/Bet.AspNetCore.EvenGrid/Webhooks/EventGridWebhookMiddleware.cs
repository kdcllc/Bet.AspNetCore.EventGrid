using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Azure.EventGrid;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Bet.AspNetCore.EvenGrid.Webhooks
{
    public class EventGridWebhookMiddleware
    {
        private readonly EventGridSubscriber _eventGridSubscriber;
        private readonly RequestDelegate _next;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<EventGridWebhookMiddleware> _logger;
        private readonly EventGridWebhooksOptions _options;

        public EventGridWebhookMiddleware(
            RequestDelegate next,
            IServiceProvider serviceProvider,
            IOptions<EventGridWebhooksOptions> options,
            ILogger<EventGridWebhookMiddleware> logger)
        {
            _next = next;
            _serviceProvider = serviceProvider;
            _logger = logger;
            _options = options.Value;

            _eventGridSubscriber = _options.GetEventGridSubscriber();
        }

        public async Task InvokeAsync(HttpContext context)
        {
            using (var streamReader = new HttpRequestStreamReader(context.Request.Body, Encoding.UTF8))
            {
                var requestContent = await streamReader.ReadToEndAsync();

                if (_logger.IsEnabled(LogLevel.Debug))
                {
                    _logger.LogDebug($"New Event Grid Webhook Message Received.{Environment.NewLine}Body: {{MessageBody}}", requestContent);
                }
                else
                {
                    _logger.LogInformation("New Event Grid Webhook Message Received.");
                }

                var response = context.Response;

                var eventGridEvents = _eventGridSubscriber.DeserializeEventGridEvents(requestContent);

                foreach (var eventGridEvent in eventGridEvents)
                {
                    try
                    {
                        if (eventGridEvent.Data is SubscriptionValidationEventData eventData)
                        {
                            var responseData = new SubscriptionValidationResponse()
                            {
                                ValidationResponse = eventData.ValidationCode
                            };

                            response.ContentType = "application/json";
                            response.StatusCode = StatusCodes.Status200OK;
                            await response.WriteAsync(JsonConvert.SerializeObject(responseData));
                        }
                        else if (eventGridEvent.Data is StorageBlobCreatedEventData storageBlobEventData)
                        {
                            // Do we need to handle this?
                        }
                        else if (eventGridEvent.Data is EventGridWebhookEvent messageEventData)
                        {
                            var registration = _options.Registrations.First(x => x.EventType == messageEventData.GetType());

                            var service = registration.Factory(_serviceProvider);

                            EventGridWebHookResult result;

                            var data = eventGridEvent.Data as EventGridWebhookEvent;

                            result = await service.ProcessEventAsync(data);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "An error occurred processing a webhook event. Message: {MessageBody}", JsonConvert.SerializeObject(eventGridEvent));
                        throw;
                    }
                }
            }
        }
    }
}
