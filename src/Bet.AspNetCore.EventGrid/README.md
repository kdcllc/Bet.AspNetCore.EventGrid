# Bet.AspNetCore.EventGrid

[![Build status](https://ci.appveyor.com/api/projects/status/ldg53oxk7nrmroo1/branch/master?svg=true)](https://ci.appveyor.com/project/kdcllc/bet-aspnetcore-eventgrid/branch/master)
[![NuGet](https://img.shields.io/nuget/v/Bet.AspNetCore.EventGrid.svg)](https://www.nuget.org/packages?q=Bet.AspNetCore.EventGrid)
[![MyGet](https://img.shields.io/myget/kdcllc/v/Bet.AspNetCore.EventGrid.svg?label=myget)](https://www.myget.org/F/kdcllc/api/v2)

Add the following to the project

```bash
    dotnet add package Bet.AspNetCore.EventGrid
```

This is a custom middleware that provides with ability to add message processing based on their incoming type.

## Usage of the Custom Web App WebHooks

1. Register Event Handlers for custom messages in `Startup.cs`

```csharp

    public void ConfigureServices(IServiceCollection services)
    {
      services.AddEventGridWebhooks()
          .AddWebhook<EmployeeWebhook, EmployeeCreatedEvent>("Group.Employee");

      // or
      var builder = services.AddEventGridWebhooks();
      builder.AddWebhook<EmployeeWebhook, EmployeeCreatedEvent>("Group.Employee");
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseEventGridWebHooks();
    }
```

2. WebHook Implementation

```csharp

    public class EmployeeWebhook : IWebhook<EmployeeCreatedEvent>
    {
        private readonly ILogger<EmployeeWebhook> _logger;

        public EmployeeWebhook(ILogger<EmployeeWebhook> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public Task<WebHookResult> ProcessEventAsync(
            EmployeeCreatedEvent webHookEvent,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Executing: {data} ", webHookEvent);

            return Task.FromResult(new WebHookResult());
        }
    }
```

For more information please refer to [Sample Web Application](../Bet.AspNetCore.EventGrid.WebApp/README.md)