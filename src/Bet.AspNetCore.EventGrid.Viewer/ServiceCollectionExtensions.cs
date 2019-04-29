using Bet.AspNetCore.EventGrid.Viewer;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddEventGridViewer(this IServiceCollection services)
        {
            services.ConfigureOptions<DefaultUIConfigureOptions>();
            return services;
        }
    }
}
