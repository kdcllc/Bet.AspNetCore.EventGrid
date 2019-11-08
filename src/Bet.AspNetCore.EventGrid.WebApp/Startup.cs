using System;

using Bet.AspNetCore.EventGrid.WebApp.Events;
using Bet.AspNetCore.EventGrid.WebApp.Handler;
using Bet.AspNetCore.EventGrid.WebApp.Middleware;
using Bet.AspNetCore.EventGrid.WebApp.Services;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Bet.AspNetCore.EventGrid.WebApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<IOperationTransient, Operation>();
            services.AddScoped<IOperationScoped, Operation>();
            services.AddSingleton<IOperationSingleton, Operation>();
            services.AddSingleton<IOperationSingletonInstance>(new Operation(Guid.Empty));

            // OperationService depends on each of the other Operation types.
            services.AddTransient<IOperationService, OperationService>();

            services.AddEvenGridWebhooks()
                .AddViewerSignalRHubContext("/hubs/events")
                .AddWebhook<EmployeeWebhook, EmployeeCreatedEvent>("Group.Employee")
                .AddWebhook<CustomerWebhook, CustomerCreatedEvent>("Group.Employee");

            services.AddMvc(r => r.EnableEndpointRouting = false).AddNewtonsoftJson();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            var logger = loggerFactory.CreateLogger<Startup>();

            Action<RequestProfilerModel> requestResponseHandler = requestProfilerModel =>
            {
                logger.LogInformation(requestProfilerModel.Request);
                logger.LogInformation(Environment.NewLine);
                logger.LogInformation(requestProfilerModel.Response);
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

            app.UseStaticFiles();

            app.UseEventGridWebHooks();

            app.UseHttpsRedirection();

            app.UseMvc();
        }
    }
}
