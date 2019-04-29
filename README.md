# Bet.AspNetCore.EventGrid solution

The middleware and viewer for Azure Event Grid.

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


## Reference material

1. [Azure Event Grid Viewer with ASP.NET Core and SignalR](https://madeofstrings.com/2018/03/14/azure-event-grid-viewer-with-asp-net-core-and-signalr/)
2. [Receive events to an HTTP endpoint](https://docs.microsoft.com/en-us/azure/event-grid/receive-events)
