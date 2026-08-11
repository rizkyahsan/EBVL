using System.Net;
using Azure.Monitor.OpenTelemetry.AspNetCore;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace EBVL.FrontEnd.Infrastructure.Monitoring;

public static class ConfigureMonitoring
{
    public static IServiceCollection AddMonitoringService(this IServiceCollection services, string appNickName, string connectionString)
    {
        var resourceAttributes = new Dictionary<string, object>
        {
            { "service.namespace", appNickName },
            { "service.name", nameof(FrontEnd) },
            { "service.instance.id", Dns.GetHostName() }
        };

        _ = services
            .AddOpenTelemetry()
            .WithTracing(builder =>
            {
                _ = builder.AddAspNetCoreInstrumentation(options =>
                {
                    options.Filter = httpContext =>
                    {
                        var path = httpContext.Request.Path.Value ?? string.Empty;

                        var isBlazor = path.Contains("_blazor");
                        var isContent = path.Contains("_content");
                        var isStatic = path.EndsWith(".js") ||
                            path.EndsWith(".ttf") ||
                            path.EndsWith(".woff") ||
                            path.EndsWith(".woff2") ||
                            path.EndsWith(".css") ||
                            path.EndsWith(".ico") ||
                            path.EndsWith(".svg") ||
                            path.EndsWith(".png") ||
                            path.EndsWith(".jpg") ||
                            path.EndsWith(".gif");

                        return !(isBlazor || isContent || isStatic);
                    };
                })
                .AddProcessor(new ActivityFilteringProcessor());
            })
            .UseAzureMonitor(x => x.ConnectionString = connectionString)
            .ConfigureResource(resourceBuilder => resourceBuilder.AddAttributes(resourceAttributes));

        return services;
    }
}
