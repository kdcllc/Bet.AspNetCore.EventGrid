# Bet.AspNetCore.EventGrid.MessageHandlers

[![Build status](https://ci.appveyor.com/api/projects/status/ldg53oxk7nrmroo1/branch/master?svg=true)](https://ci.appveyor.com/project/kdcllc/bet-aspnetcore-eventgrid/branch/master)
[![NuGet](https://img.shields.io/nuget/v/Bet.AspNetCore.EventGrid.MessageHandlers.svg)](https://www.nuget.org/packages?q=Bet.AspNetCore.EventGrid.MessageHandlers)
[![MyGet](https://img.shields.io/myget/kdcllc/v/Bet.AspNetCore.EventGrid.MessageHandlers.svg?label=myget)](https://www.myget.org/F/kdcllc/api/v2)

Add the following to the project

```bash
    dotnet add package Bet.AspNetCore.EventGrid.MessageHandlers
```

Sending Event Grid Events with SAS Authorization Handler

Add the registration in `Startup.cs` or `Program.cs`:

```csharp
     // add http client
     services.AddCloudEventClient("BetCloudEventClient", endpoint, key, TimeSpan.FromMinutes(10));
```

Inject the custom `ICloudEventClient` into the service and use it

```csharp
    private readonly ICloudEventClient _cloudEventClient;

    public EventService(ICloudEventClient cloudEventClient)
    {
        _cloudEventClient = cloudEventClient ?? throw new ArgumentNullException(nameof(cloudEventClient));
    }

    public async Task SendCloudEvent()
    {
        var data = new EmployeeCreatedEvent
        {
            Id = Guid.NewGuid().ToString(),
            Name = "CloudEvent Test"
        };

        var cloudEvent = new CloudEvent<EmployeeCreatedEvent>(data, "Group.Employee");

        await _cloudEventClient.SendEventAsync(cloudEvent, _applicationLifetime.ApplicationStopping);
    }
```
