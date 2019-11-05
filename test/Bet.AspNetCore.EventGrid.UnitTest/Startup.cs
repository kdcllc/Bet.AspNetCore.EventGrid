using Bet.AspNetCore.EvenGrid.Internal;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Bet.AspNetCore.EventGrid.UnitTest
{
    internal class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddEvenGridWebhooks()
              .AddWebhook<EmployeeWebhook, EmployeeCreatedEvent>("Group.Employee");
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseEventGridWebHooks();
        }
    }
}
