using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Bet.AspNetCore.EvenGrid.Models;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Azure.EventGrid;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Bet.AspNetCore.EvenGrid.Webhooks
{
    internal class EventGridWebhookMiddleware : IMiddleware
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IHostingEnvironment _enviroment;
        private readonly ILogger<EventGridWebhookMiddleware> _logger;
        private readonly EventGridWebhooksOptions _options;
        private readonly IHubContext<GridEventsHub> _hubContext;

        public EventGridWebhookMiddleware(
            IServiceProvider serviceProvider,
            IHostingEnvironment enviroment,
            IOptions<EventGridWebhooksOptions> options,
            IHubContext<GridEventsHub> gridEventsHubContext,
            ILogger<EventGridWebhookMiddleware> logger)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _enviroment = enviroment;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _options = options.Value;

            _hubContext = gridEventsHubContext;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (context.Request.Path != _options.HttpRoute
                || context.Request.Method != _options.HttpMethod)
            {
                await next(context);
                return;
            }

            using (var streamReader = new HttpRequestStreamReader(context.Request.Body, Encoding.UTF8))
            {
                var jsonContent = await streamReader.ReadToEndAsync();

                if (_logger.IsEnabled(LogLevel.Debug))
                {
                    _logger.LogDebug($"New Event Grid Webhook Message Received.{Environment.NewLine}Body: {{MessageBody}}", jsonContent);
                }
                else
                {
                    _logger.LogInformation("New Event Grid Webhook Message Received.");
                }

                var cts = new CancellationTokenSource();

                try
                {
                    var response = context.Response;

                    var tasksExeptions = new List<EventGridWebHookResult>();

                    if (_options.EventTypeSubcriptionValidation(context))
                    {
                        var validationResult = await HandleValidation(jsonContent);
                        response.ContentType = "application/json";
                        response.StatusCode = StatusCodes.Status200OK;
                        await response.WriteAsync(validationResult);
                    }
                    else if (_options.EventTypeNotification(context))
                    {
                        if (IsCloudEvent(jsonContent))
                        {
                            tasksExeptions = await HandleCloudEvent(jsonContent, cts.Token);
                        }
                        else
                        {
                            tasksExeptions = await HandleGridEvents(jsonContent, cts.Token);
                        }

                        if (tasksExeptions.Count > 0
                            && !_enviroment.IsDevelopment()
                            && !_options.ThrowIfException)
                        {
                            response.ContentType = "application/json";
                            response.StatusCode = StatusCodes.Status500InternalServerError;
                            await response.WriteAsync(string.Empty);
                        }
                        else if (_options.ThrowIfException && tasksExeptions.Count > 0)
                        {
                            throw new AggregateException($"{nameof(EventGridWebhookMiddleware)} raised exceptions.", tasksExeptions.Select(x => x.Exception));
                        }
                    }
                }
                finally
                {
                    cts?.Dispose();
                }
            }
        }

        private async Task<string> HandleValidation(string jsonContent)
        {
            var gridEvent = JsonConvert.DeserializeObject<List<GridEvent<Dictionary<string, string>>>>(jsonContent).First();

            if (_options.ViewerHubContextEnabled)
            {
                await _hubContext.Clients.All.SendAsync(
                    "gridupdate",
                    gridEvent.Id,
                    gridEvent.EventType,
                    gridEvent.Subject,
                    gridEvent.EventTime.ToLongTimeString(),
                    jsonContent,
                    "success");
            }

            // Retrieve the validation code and echo back.
            var validationCode = gridEvent.Data["validationCode"];

            return JsonConvert.SerializeObject(new
            {
                validationResponse = validationCode
            });
        }

        private async Task<List<EventGridWebHookResult>> HandleGridEvents(string jsonContent, CancellationToken token)
        {
            var tasksExeptions = new List<EventGridWebHookResult>();

            var jToken = JToken.Parse(jsonContent);

            if (jToken is JArray)
            {
                var events = JArray.Parse(jsonContent);
                foreach (var jtEvent in events)
                {
                    var result = await ProcessGridEvent(jtEvent, token);
                    if (result != null)
                    {
                        tasksExeptions.Add(result);
                    }
                }
            }
            else if (jToken is JObject)
            {
                var result = await ProcessGridEvent(jToken, token);
                if (result != null)
                {
                    tasksExeptions.Add(result);
                }
            }

            return tasksExeptions;
        }

        private async Task<EventGridWebHookResult> ProcessGridEvent(JToken jtEvent, CancellationToken token)
        {
            EventGridWebHookResult result = null;
            try
            {
                var @event = JsonConvert.DeserializeObject<GridEvent<object>>(jtEvent.ToString());

                object messageEventData = null;

                var webhook = _options.WebHooksRegistrations.FirstOrDefault(x =>
                {
                    try
                    {
                        messageEventData = JsonConvert.DeserializeObject(@event.Data.ToString(), x.EventType, new JsonSerializerSettings { MissingMemberHandling = MissingMemberHandling.Error });
                    }
                    catch
                    {
                        return false;
                    }

                    return messageEventData != null && x.EventType == messageEventData.GetType() && x.EventGridType == @event.EventType;
                });

                if (webhook == null)
                {
                    throw new ArgumentException($"Can't find a Event Grid Webhook for this grid event: {@event.EventType}");
                }


                var service = webhook.Factory(_serviceProvider);

                var method = webhook.WebhookType.GetMethod("ProcessEventAsync");

                result = await (Task<EventGridWebHookResult>)method.Invoke(service, parameters: new object[] { messageEventData, token });
            }
            catch (Exception ex)
            {
              result = new EventGridWebHookResult(ex);
            }
            finally
            {
                if (_options.ViewerHubContextEnabled)
                {
                    // Invoke a method on the clients for
                    // an event grid notification.
                    var details = JsonConvert.DeserializeObject<GridEvent<dynamic>>(jtEvent.ToString());
                    await _hubContext.Clients.All.SendAsync(
                        "gridupdate",
                        details.Id,
                        details.EventType,
                        details.Subject,
                        details.EventTime.ToLongTimeString(),
                        jtEvent.ToString(),
                        result?.Exception.Message ?? "success");
                }
            }

            if (result?.Exception != null)
            {
                return result;
            }
            return null;
        }

        private async Task<List<EventGridWebHookResult>> HandleCloudEvent(string jsonContent, CancellationToken token)
        {
            var tasksExeptions = new List<EventGridWebHookResult>();

            EventGridWebHookResult result = null;

            try
            {
                var @event = JsonConvert.DeserializeObject<CloudEvent<object>>(jsonContent);

                object messageEventData = null;

                var webhook = _options.WebHooksRegistrations.FirstOrDefault(x => {
                    try
                    {
                        messageEventData = JsonConvert.DeserializeObject(@event.Data.ToString(), x.EventType, new JsonSerializerSettings { MissingMemberHandling = MissingMemberHandling.Error });
                    }
                    catch
                    {
                        return false;
                    }

                    return messageEventData != null && x.EventType == messageEventData.GetType() && x.EventGridType == @event.EventType;
                });

                if (webhook == null)
                {
                    throw new ArgumentException($"Can't find a Event Grid Webhook for this grid event: {@event.EventType}");
                }

                var service = webhook.Factory(_serviceProvider);

                var method = webhook.WebhookType.GetMethod("ProcessEventAsync");

                result = await (Task<EventGridWebHookResult>)method.Invoke(service, parameters: new object[] { messageEventData, token });
            }
            catch (Exception ex)
            {
                tasksExeptions.Add(new EventGridWebHookResult(ex));
            }
            finally
            {
                if (_options.ViewerHubContextEnabled)
                {
                    var details = JsonConvert.DeserializeObject<CloudEvent<dynamic>>(jsonContent);

                    // CloudEvents schema and mapping to
                    // Event Grid: https://docs.microsoft.com/en-us/azure/event-grid/cloudevents-schema
                    await _hubContext.Clients.All.SendAsync(
                        "gridupdate",
                        details.EventId,
                        details.EventType,
                        details.Source,
                        details.EventTime,
                        jsonContent,
                        result?.Exception.Message ?? "success"
                    );
                }

                if (result?.Exception != null)
                {
                    tasksExeptions.Add(result);
                }
            }
            return tasksExeptions;
        }

        private bool IsCloudEvent(string jsonContent)
        {
            // Cloud events are sent one at a time, while Grid events
            // are sent in an array. As a result, the JObject.Parse will
            // fail for Grid events.
            try
            {
                var jToken = JToken.Parse(jsonContent);

                if (jToken is JArray)
                {
                    return false;
                }

                // Attempt to read one JSON object.
                var eventData = JObject.Parse(jsonContent);

                // Check for the cloud events version property.
                var version = eventData["cloudEventsVersion"]?.Value<string>();
                if (!string.IsNullOrEmpty(version))
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred {methodName}", nameof(IsCloudEvent));
            }

            return false;
        }

    }
}
