using Bet.AspNetCore.EventGrid.Viewer;

using Microsoft.AspNetCore.Http.Connections;

namespace Microsoft.AspNetCore.Builder
{
    public static class ViewerAspNetCoreBuilderExtensions
    {
        public static IApplicationBuilder UseEventGridWebHooksViewer(this IApplicationBuilder app, string viewerHubContextRoute)
        {
            app.UseStaticFiles();
            app.UseWebSockets();

            var desiredTransports =
                          HttpTransportType.WebSockets |
                          HttpTransportType.LongPolling;

#if NETSTANDARD2_0 || NETCOREAPP2_2

            app.UseSignalR((configure) =>
            {
                configure.MapHub<WebhooksSignalRHub>(
                    viewerHubContextRoute,
                    opt => opt.Transports = desiredTransports);
            });
#else
            app.UseFileServer();
            app.UseRouting();
            app.UseEndpoints(endpoints => endpoints.MapHub<WebhooksSignalRHub>(
                viewerHubContextRoute,
                opt => opt.Transports = desiredTransports));
#endif
            return app;
        }
    }
}
