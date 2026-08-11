using HealthChecks.UI.Client;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Pertamina.Services.HealthCheck;

namespace EBVL.BackEnd.Infrastructure.HealthCheck;

public static class ConfigureHealthCheck
{
    public static IHealthChecksBuilder AddHealthCheckService(this IServiceCollection services, IConfiguration configuration, string pathBase, string appNickName, string connectionString)
    {
        var healthCheckSection = configuration.GetRequiredSection(HealthCheckOptions.SectionKey);
        var healthCheckOptions = healthCheckSection.Get<HealthCheckOptions>()
            ?? throw ExceptionFor.ConfigurationBindingFailed(HealthCheckOptions.SectionKey, typeof(HealthCheckOptions));

        _ = services.Configure<HealthCheckOptions>(healthCheckSection);

        var uri = healthCheckOptions.Endpoint;

        if (uri.Contains(':') && !string.IsNullOrWhiteSpace(pathBase))
        {
            uri = uri.Replace("/{pathbase}", pathBase);
        }
        else if (!string.IsNullOrWhiteSpace(pathBase))
        {
            uri = $"{pathBase}{uri}";
        }

        if (healthCheckOptions.UI.Enabled)
        {
            _ = services
                .AddHealthChecksUI(settings =>
                {
                    _ = settings.AddHealthCheckEndpoint($"{appNickName} Services", uri);
                    _ = settings.SetEvaluationTimeInSeconds(healthCheckOptions.UI.PollingInterval);
                })
                .AddSqlServerStorage(connectionString, options =>
                {
                    _ = options.ConfigureWarnings(wcb => wcb.Ignore(CoreEventId.RowLimitingOperationWithoutOrderByWarning));
                    _ = options.ConfigureWarnings(wcb => wcb.Ignore(RelationalEventId.MultipleCollectionIncludeWarning));
                });
        }

        return services.AddHealthChecks();
    }

    public static IApplicationBuilder UseHealthCheckService(this WebApplication app, string pathBase)
    {
        var healthCheckOptions = app.Configuration.GetRequiredSection(HealthCheckOptions.SectionKey).Get<HealthCheckOptions>()
            ?? throw ExceptionFor.ConfigurationBindingFailed(HealthCheckOptions.SectionKey, typeof(HealthCheckOptions));

        var endpoint = healthCheckOptions.Endpoint;

        if (healthCheckOptions.Endpoint.Contains(':'))
        {
            var uri = new Uri(healthCheckOptions.Endpoint);
            var lastSegment = uri.Segments[^1];
            endpoint = $"/{lastSegment}";
        }

        if (!string.IsNullOrWhiteSpace(pathBase))
        {
            endpoint = $"{pathBase}{endpoint}";
        }

        _ = app.MapHealthChecks(endpoint, new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
        {
            Predicate = _ => true,
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        });

        if (app.Logger.IsEnabled(LogLevel.Information))
        {
            app.Logger.LogInformation("This web application is configured with Health Checks at: {Endpoint}.", healthCheckOptions.Endpoint);
        }

        if (healthCheckOptions.UI.Enabled)
        {
            _ = app.MapHealthChecksUI(ui =>
            {
                ui.UIPath = healthCheckOptions.UI.Endpoints.UI;
                ui.ApiPath = healthCheckOptions.UI.Endpoints.Api;
                _ = ui.AddCustomStylesheet("wwwroot/healthchecks/site.css");
            });
        }

        if (app.Logger.IsEnabled(LogLevel.Information))
        {
            app.Logger.LogInformation("This web application is configured with Health Check UI at: {Endpoint}.", healthCheckOptions.UI.Endpoints.UI);
        }

        return app;
    }
}
