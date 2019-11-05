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
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Bet.AspNetCore.EvenGrid.Internal
{
    internal class WebhookMiddleware : IMiddleware
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IHostingEnvironment _enviroment;
        private readonly ILogger<WebhookMiddleware> _logger;
        private readonly WebhooksOptions _options;
        private readonly IHubContext<WebhooksSignalRHub> _hubContext;

        public WebhookMiddleware(
            IServiceProvider serviceProvider,
            IHostingEnvironment enviroment,
            IOptions<WebhooksOptions> options,
            IHubContext<WebhooksSignalRHub> gridEventsHubContext,
            ILogger<WebhookMiddleware> logger)
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

            var response = context?.Response;

            if (context?.Request.Path != _options.HttpRoute
                || context.Request.Method != _options.HttpMethod
                || response == null)
            {
                await next(context);
                return;
            }

            // create a new cancelation token
            var cancellationToken = context?.RequestAborted ?? CancellationToken.None;
            var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            try
            {
                using (var streamReader = new HttpRequestStreamReader(context?.Request.Body, Encoding.UTF8))
                {
                    using var jsonReader = new JsonTextReader(streamReader);
                    var jToken = await JToken.LoadAsync(jsonReader, cts.Token);

                    _logger.LogInformation("New Event Grid Webhook Message Received.");

                    var tasksExeptions = new List<WebHookResult>();

                    if (_options.EventTypeSubscriptionValidation(context!))
                    {
                        var validationResult = await HandleValidation(jToken);
                        response.ContentType = "application/json";
                        response.StatusCode = StatusCodes.Status200OK;
                        await response.WriteAsync(validationResult);
                    }
                    else if (_options.EventTypeNotification(context!))
                    {
                        var events = GetEvents(jToken);

                        if (events != null)
                        {
                            tasksExeptions = await ProcessEvents(events, cts.Token);
                        }
                        else
                        {
                            tasksExeptions.Add(new WebHookResult(new Exception("No Webhook Events were found")));
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
                            throw new AggregateException($"{nameof(WebhookMiddleware)} raised exceptions.", tasksExeptions.Select(x => x.Exception));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                cts.Cancel();
                _logger.LogError(ex, string.Empty);
                throw;
            }
            finally
            {
                cts?.Dispose();
            }
        }

        private async Task<string> HandleValidation(JToken jToken)
        {
            var jsonContent = jToken.ToString();

            var gridEvent = JsonConvert.DeserializeObject<List<GridEvent<Dictionary<string, string>>>>(jsonContent).First();

            if (_options.ViewerHubContextEnabled)
            {
                await _hubContext.Clients.All.SendAsync(
                    "gridupdate",
                    gridEvent.Id,
                    gridEvent.EventType,
                    gridEvent.Subject,
                    gridEvent.EventTime.DateTime.ToLongTimeString(),
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

        private async Task<List<WebHookResult>> ProcessEvents(JArray events, CancellationToken token)
        {
            var tasksExeptions = new List<WebHookResult>();

            foreach (var jtEvent in events)
            {
                var result = await ProcessSingleEvent(jtEvent, token);
                if (result != null)
                {
                    tasksExeptions.Add(result);
                }
            }

            return tasksExeptions;
        }

        private async Task<WebHookResult?> ProcessSingleEvent(JToken jtToken, CancellationToken token)
        {
            WebHookResult? result = null;
            try
            {
                var singleEvent = JsonConvert.DeserializeObject<GridEvent<object>>(jtToken.ToString());

                object? messageEventData = null;

                var webhook = _options.WebHooksRegistrations.FirstOrDefault(x =>
                {
                    try
                    {
                        messageEventData = JsonConvert.DeserializeObject(
                            singleEvent.Data.ToString(),
                            x.EventType,
                            new JsonSerializerSettings { MissingMemberHandling = MissingMemberHandling.Error });
                    }
                    catch
                    {
                        return false;
                    }

                    return messageEventData != null
                        && x.EventType == messageEventData.GetType()
                        && x.EventGridType == singleEvent.EventType;
                });

                if (webhook == null)
                {
                    throw new ArgumentException($"Can't find a Event Grid Webhook for this grid event: {singleEvent.EventType}");
                }

                var service = webhook.Factory(_serviceProvider);

                var method = webhook.WebhookType.GetMethod("ProcessEventAsync");

                result = await (Task<WebHookResult>)method.Invoke(service, parameters: new object[] { messageEventData, token });
            }
            catch (Exception ex)
            {
                result = new WebHookResult(ex);
            }
            finally
            {
                if (_options.ViewerHubContextEnabled)
                {
                    // Invoke a method on the clients for
                    // an event grid notification.
                    var details = JsonConvert.DeserializeObject<GridEvent<dynamic>>(jtToken.ToString());
                    await _hubContext.Clients.All.SendAsync(
                        "gridupdate",
                        details.Id,
                        details.EventType,
                        details.Subject,
                        details.EventTime.DateTime.ToLongTimeString(),
                        jtToken.ToString(),
                        result?.Exception?.Message ?? "success");
                }
            }

            if (result?.Exception != null)
            {
                return result;
            }

            return null;
        }

        private JArray? GetEvents(JToken inputJtoken)
        {
            if (inputJtoken is JArray)
            {
                return inputJtoken as JArray;
            }
            else if (inputJtoken is JObject)
            {
                var jarr = new JArray
                {
                    inputJtoken
                };

                return jarr;
            }

            return null;
        }
    }
}
