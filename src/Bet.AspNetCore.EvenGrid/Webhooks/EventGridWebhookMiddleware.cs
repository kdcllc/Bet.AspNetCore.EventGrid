using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
    internal class EventGridWebhookMiddleware
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
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (context.Request.Path != _options.HttpRoute
                || context.Request.Method != _options.HttpMethod)
            {
                await _next(context);
                return;
            }

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

                var tasksExeptions = new List<EventGridWebHookResult>();

                var cts = new CancellationTokenSource();

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
                            var webhook = _options.WebHooksRegistrations.First(x => x.EventType == messageEventData.GetType());

                            var service = webhook.Factory(_serviceProvider);

                            var method = webhook.WebhookType.GetMethod("ProcessEventAsync");

                            var result = await (Task<EventGridWebHookResult>)method.Invoke(service, parameters: new object[] { messageEventData, cts.Token });

                            if (result.Exception != null)
                            {
                                _logger.LogError(result.Exception.Message, "");
                                tasksExeptions.Add(result);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        if (_logger.IsEnabled(LogLevel.Debug))
                        {
                            _logger.LogError(ex, "An error occurred processing an Event Grid Webhook Message: {MessageBody}", JsonConvert.SerializeObject(eventGridEvent));
                        }
                        else
                        {
                            _logger.LogInformation("An error occurred processing an Event Grid Webhook Message.");
                        }
                        tasksExeptions.Add(new EventGridWebHookResult(ex));
                    }
                    finally
                    {
                        cts?.Dispose();
                    }

                    if (_options.ThrowIfException
                        && tasksExeptions.Count > 0)
                    {
                        throw new AggregateException($"{nameof(EventGridWebhookMiddleware)} raised exceptions.", tasksExeptions.Select(x=> x.Exception));
                    }
                }
            }
        }
    }
}
