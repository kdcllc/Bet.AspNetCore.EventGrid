using System;

using Bet.AspNetCore.EventGrid.WebApp.Events;
using Bet.AspNetCore.EventGrid.WebApp.Handler;
using Bet.AspNetCore.EventGrid.WebApp.Services;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;

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
            services.AddHealthChecks().AddCheck("simple_check", (token) => new HealthCheckResult(HealthStatus.Healthy));

            services.AddTransient<IOperationTransient, Operation>();
            services.AddScoped<IOperationScoped, Operation>();
            services.AddSingleton<IOperationSingleton, Operation>();
            services.AddSingleton<IOperationSingletonInstance>(new Operation(Guid.Empty));

            // OperationService depends on each of the other Operation types.
            services.AddTransient<IOperationService, OperationService>();

            services.AddEventGridWebhooks()
                .AddDiagnostics("/check")
                .AddViewerSignalRHubContext("/hubs/events")
                .AddWebhook<EmployeeWebhook, EmployeeCreatedEvent>("Group.Employee")
                .AddWebhook<CustomerWebhook, CustomerCreatedEvent>("Group.Employee");

            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            });

            services.AddControllers().AddNewtonsoftJson();

            services.AddDeveloperListRegisteredServices();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseRequestResponseLogging();
                app.UseDeveloperListRegisteredServices();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseStaticFiles();

            app.UseEventGridWebHooks();

            var enaledSSL = Configuration.GetValue<bool>("EnabledSSL");
            if (enaledSSL)
            {
                app.UseHttpsRedirection();
            }

            app.UseHealthyHealthCheck();
            app.UseLivenessHealthCheck();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapRazorPages();
            });
        }
    }
}
