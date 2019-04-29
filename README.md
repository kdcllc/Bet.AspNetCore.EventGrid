# Bet.AspNetCore.EventGrid solution

The middleware and viewer for Azure Event Grid. 


## Required Azure resources:

1. Azure Event Grid Topic
2. Azure Web App or App Functions


## Usage of the Azure Web App WebHooks

1. Register Event Handlers

```csharp

    services.AddEvenGridWebhooks()
        .AddWebhook<EmployeeWebhook, EmployeeCreatedEvent>("Group.Employee")
        .AddWebhook<CustomerWebhook, CustomerCreatedEvent>("Group.Employee");
```

2. Add Middleware

```csharp

   app.UseEventGridHooks();

```

## Test Data for Bet.AspNetCore.EventGrid.Viewer project

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

## Reference material

1. [Azure Event Grid Viewer with ASP.NET Core and SignalR](https://madeofstrings.com/2018/03/14/azure-event-grid-viewer-with-asp-net-core-and-signalr/)
2. [Receive events to an HTTP endpoint](https://docs.microsoft.com/en-us/azure/event-grid/receive-events)
