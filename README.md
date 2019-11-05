# Bet.AspNetCore.EventGrid solution
[![Build status](https://ci.appveyor.com/api/projects/status/ldg53oxk7nrmroo1/branch/master?svg=true)](https://ci.appveyor.com/project/kdcllc/bet-aspnetcore-eventgrid/branch/master)
[![NuGet](https://img.shields.io/nuget/v/Bet.AspNetCore.EvenGrid.svg)](https://www.nuget.org/packages?q=Bet.AspNetCore.EvenGrid)
[![MyGet](https://img.shields.io/myget/kdcllc/v/Bet.AspNetCore.EvenGrid.svg?label=myget)](https://www.myget.org/F/kdcllc/api/v2)

The middleware and viewer for Azure Event Grid.

## Required Azure resources

1. Azure Event Grid Topic
2. Azure Web App or App Functions
3. Azure App Web Service

## Usage of the Azure Web App WebHooks

1. Register Event Handlers

```csharp

    services.AddEvenGridWebhooks()
                .AddViewerHubContext()
                .AddWebhook<EmployeeWebhook, EmployeeCreatedEvent>("Group.Employee")
                .AddWebhook<CustomerWebhook, CustomerCreatedEvent>("Group.Employee");

    // enable Azure Grid Event Viewer
    services.AddEventGridViewer();
```

2. Add Middleware

```csharp

   app.UseEventGridWebHooks();

```

3. WebHook implementation example

```csharp

public class CustomerWebhook : IEventGridWebhook<CustomerCreatedEvent>
    {
        private readonly ILogger<CustomerWebhook> _logger;

        public CustomerWebhook(ILogger<CustomerWebhook> logger)
        {
            _logger = logger;
        }

        public Task<EventGridWebHookResult> ProcessEventAsync(CustomerCreatedEvent @event, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Executing: {data} ", @event);

            return Task.FromResult<EventGridWebHookResult>(null);
        }
    }
```

4. Azure Event Grid Viewer

To see messages coming in to the middleware.

> https://localhost:5001/events/viewer

## Test Data for Bet.AspNetCore.EventGrid.WebApp project

```json
[
  {
    "topic": "/subscriptions/1234/resourceGroups/test-rs-gp/providers/Microsoft.EventGrid/topics/test-eg-topic",
    "subject": "Bet-Dream-Func",
    "id": "ab8d8ef9-b929-46d8-b205-d72113750da1",
    "eventType": "Group.Employee",
    "eventTime": "2019-04-28T18:51:17.7994409Z",
    "data":{
    	"Id": "12345",
    	"Name": "hello"
    },
    "dataVersion": "2",
    "metadataVersion": "1"
  }
]
```

## Event Grid Http Message Hander for SAS Token Authentication

```csharp

 var client = new HttpClient(new SasAuthorizeMessageHandler(new SasAuthorizeOptions(
                Endpoint, Key, TimeSpan.FromMinutes(8))) { InnerHandler = new HttpClientHandler() });
```

## Testing Azure Event Grid Webhooks callbacks
What if you want to test the round trip communication locally on your machine. [`ngrok`](https://ngrok.com/?source=kdcllc) to the rescue.

1. Install with `npm i -g ngrok`.
2. Run `Bet.AspNetCore.EventGrid.WebApp` project on port `5201`
3. Run `ngrok http https://localhost:5201` (to test function locally run `ngrok http -host-header=localhost 7071`)
![ngrok](./img/ngrok.server.jpg)
4. Use the randomly generated URL for EventGrid Webhook. Create Event Subscription
![eventgrid topic](./img/eventgrid-topic.jpg)

## Reference material

1. [Azure Event Grid Viewer with ASP.NET Core and SignalR](https://madeofstrings.com/2018/03/14/azure-event-grid-viewer-with-asp-net-core-and-signalr/)

2. [Receive events to an HTTP endpoint](https://docs.microsoft.com/en-us/azure/event-grid/receive-events)

3. [Locally debugging an Azure Function triggered by Azure Event Grid](https://blogs.msdn.microsoft.com/brandonh/2017/11/30/locally-debugging-an-azure-function-triggered-by-azure-event-grid/)

4. [Azure Event Grid event schema](https://docs.microsoft.com/en-us/azure/event-grid/event-schema)

```json
[
  {
    "topic": "/subscriptions/{subscription-id}/resourceGroups/Storage/providers/Microsoft.Storage/storageAccounts/xstoretestaccount",
    "subject": "/blobServices/default/containers/oc2d2817345i200097container/blobs/oc2d2817345i20002296blob",
    "eventType": "Microsoft.Storage.BlobCreated",
    "eventTime": "2017-06-26T18:41:00.9584103Z",
    "id": "831e1650-001e-001b-66ab-eeb76e069631",
    "data": {
      "api": "PutBlockList",
      "clientRequestId": "6d79dbfb-0e37-4fc4-981f-442c9ca65760",
      "requestId": "831e1650-001e-001b-66ab-eeb76e000000",
      "eTag": "0x8D4BCC2E4835CD0",
      "contentType": "application/octet-stream",
      "contentLength": 524288,
      "blobType": "BlockBlob",
      "url": "https://oc2d2817345i60006.blob.core.windows.net/oc2d2817345i200097container/oc2d2817345i20002296blob",
      "sequencer": "00000000000004420000000000028963",
      "storageDiagnostics": {
        "batchId": "b68529f3-68cd-4744-baa4-3c0498ec19f0"
      }
    },
    "dataVersion": "",
    "metadataVersion": "1"
  }
]

```
5. [CloudEvent schema](https://docs.microsoft.com/en-us/azure/event-grid/cloudevents-schema#cloudevent-schema)

```json
{
    "cloudEventsVersion" : "0.1",
    "eventType" : "Microsoft.Storage.BlobCreated",
    "eventTypeVersion" : "",
    "source" : "/subscriptions/{subscription-id}/resourceGroups/{resource-group}/providers/Microsoft.Storage/storageAccounts/{storage-account}#blobServices/default/containers/{storage-container}/blobs/{new-file}",
    "eventID" : "173d9985-401e-0075-2497-de268c06ff25",
    "eventTime" : "2018-04-28T02:18:47.1281675Z",
    "data" : {
      "api": "PutBlockList",
      "clientRequestId": "6d79dbfb-0e37-4fc4-981f-442c9ca65760",
      "requestId": "831e1650-001e-001b-66ab-eeb76e000000",
      "eTag": "0x8D4BCC2E4835CD0",
      "contentType": "application/octet-stream",
      "contentLength": 524288,
      "blobType": "BlockBlob",
      "url": "https://oc2d2817345i60006.blob.core.windows.net/oc2d2817345i200097container/oc2d2817345i20002296blob",
      "sequencer": "00000000000004420000000000028963",
      "storageDiagnostics": {
        "batchId": "b68529f3-68cd-4744-baa4-3c0498ec19f0"
      }
    }
}
```