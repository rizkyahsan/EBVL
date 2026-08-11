using Hangfire;
using Hangfire.Dashboard.BasicAuthorization;
using Hangfire.SqlServer;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using Pertamina.Services.BackgroundJob;
using Pertamina.Services.BackgroundJob.Hangfire;

namespace EBVL.BackEnd.Infrastructure.BackgroundJob;

public static class ConfigureBackgroundJob
{
    private static readonly JsonSerializerSettings _jsonSerializerSettings = new()
    {
        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
    };

    public static IServiceCollection AddBackgroundJobService(this IServiceCollection services, IConfiguration configuration, string connectionString, IHealthChecksBuilder healthChecksBuilder)
    {
        var backgroundJobOptions = configuration.GetRequiredSection(BackgroundJobOptions.SectionKey).Get<BackgroundJobOptions>()
            ?? throw ExceptionFor.ConfigurationBindingFailed(BackgroundJobOptions.SectionKey, typeof(BackgroundJobOptions));

        JobStorage.Current = new SqlServerStorage(connectionString);

        _ = services.AddHangfire(configuration =>
        {
            _ = configuration.UseSerilogLogProvider();
            _ = configuration.UseSqlServerStorage(connectionString);
            _ = configuration.UseSerializerSettings(_jsonSerializerSettings);
            _ = configuration.UseSimpleAssemblyNameTypeSerializer();
            _ = configuration.UseRecommendedSerializerSettings();
        });

        _ = services.AddHangfireServer(options =>
        {
            options.WorkerCount = backgroundJobOptions.WorkerCount;
            options.CancellationCheckInterval = TimeSpan.FromSeconds(1);
        });

        _ = services.AddScoped<IBackgroundJobService, BackgroundJobService>();

        _ = healthChecksBuilder.AddHangfire(
            setup => setup.MinimumAvailableServers = 1,
            name: $"Background Job Service: {nameof(Hangfire)}",
            tags: ["Background Job"]);

        var builder = new SqlConnectionStringBuilder(connectionString);
        var databaseName = builder.InitialCatalog;

        _ = healthChecksBuilder.AddSqlServer(
            connectionString: connectionString,
            name: $"Background Job Database: SQL Server ({databaseName})",
            tags: ["Database"]);

        return services;
    }

    public static IApplicationBuilder UseBackgroundJobService(this WebApplication app, string pathBase, string username, string password)
    {
        var backgroundJobOptions = app.Configuration.GetRequiredSection(BackgroundJobOptions.SectionKey).Get<BackgroundJobOptions>()
            ?? throw ExceptionFor.ConfigurationBindingFailed(BackgroundJobOptions.SectionKey, typeof(BackgroundJobOptions));

        var options = new DashboardOptions
        {
            AppPath = $"{pathBase}/scalar",
            Authorization = new[]
            {
                new BasicAuthAuthorizationFilter(new BasicAuthAuthorizationFilterOptions
                {
                    RequireSsl = false,
                    SslRedirect = false,
                    LoginCaseSensitive = true,
                    Users = new []
                    {
                        new BasicAuthAuthorizationUser
                        {
                            Login = username,
                            PasswordClear =  password
                        }
                    }
                })
            }
        };

        _ = app.UseHangfireDashboard(backgroundJobOptions.DashboardUrl, options);

        app.Logger.LogInformation("This web application is configured with Background Job at: {Endpoint}.", backgroundJobOptions.DashboardUrl);

        return app;
    }
}
