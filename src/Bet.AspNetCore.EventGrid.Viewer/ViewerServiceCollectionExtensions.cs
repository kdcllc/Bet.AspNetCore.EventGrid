using Bet.AspNetCore.EventGrid.Viewer;
#if NETSTANDARD2_0
using Newtonsoft.Json.Serialization;
#endif

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ViewerServiceCollectionExtensions
    {
        public static IServiceCollection AddViewerSignalR(
            this IServiceCollection services,
            string httpRoute = "/hubs/gridevents")
        {
#if NETSTANDARD2_0
            services.AddSignalR()
                .AddJsonProtocol(options =>
            {
                options.PayloadSerializerSettings.ContractResolver =
                new DefaultContractResolver();
            });
#else
            services.AddSignalR();
            services.AddRazorPages();
#endif
            services.ConfigureOptions<DefaultUIConfigureOptions>();

            services.Configure<ViewerOptions>(o =>
            {
                o.Route = httpRoute;
            });

            return services;
        }
    }
}
