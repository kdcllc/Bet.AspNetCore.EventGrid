using Bet.AspNetCore.EventGrid.Abstractions.Internal.Tests;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Benchmarks
{
    internal class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddEventGridWebhooks()
              .AddWebhook<EmployeeWebhook, EmployeeCreatedEvent>("Group.Employee");
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseEventGridWebHooks();
        }
    }
}
