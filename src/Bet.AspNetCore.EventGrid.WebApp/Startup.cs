using System;

using Bet.AspNetCore.EventGrid.WebApp.Events;
using Bet.AspNetCore.EventGrid.WebApp.Handler;
using Bet.AspNetCore.EventGrid.WebApp.Middleware;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Bet.AspNetCore.EventGrid.WebApp
{
    public class Startup
    {
        private readonly ILogger<Startup> _logger;

        public Startup(IConfiguration configuration, ILogger<Startup> logger)
        {
            Configuration = configuration;
            _logger = logger;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddEvenGridWebhooks()
                .AddViewerHubContext("/hubs/grid")
                .AddWebhook<EmployeeWebhook, EmployeeCreatedEvent>("Group.Employee")
                .AddWebhook<CustomerWebhook, CustomerCreatedEvent>("Group.Employee");

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            Action<RequestProfilerModel> requestResponseHandler = requestProfilerModel =>
            {
                _logger.LogInformation(requestProfilerModel.Request);
                _logger.LogInformation(Environment.NewLine);
                _logger.LogInformation(requestProfilerModel.Response);
            };
            app.UseMiddleware<RequestResponseLoggingMiddleware>(requestResponseHandler);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseEventGridWebHooks();

            app.UseHttpsRedirection();

            app.UseMvc();
        }
    }
}
