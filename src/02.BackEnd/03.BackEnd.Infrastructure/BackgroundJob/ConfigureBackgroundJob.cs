using EBVL.BackEnd.Infrastructure.BackgroundJob.Schedulers.Project;
using EBVL.BackEnd.Logics.Common.Services.LogEmailDb;
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

    public static async Task EnsureLocalBackgroundJobDatabaseAsync(string connectionString)
    {
        var connectionStringBuilder = new SqlConnectionStringBuilder(connectionString);

        // EF Core creates the application/identity databases during migration, but
        // Hangfire only creates its schema. Provision its database for LocalDB
        // development without changing Azure/production database lifecycle.
        if (!connectionStringBuilder.DataSource.StartsWith("(localdb)\\", StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        var databaseName = connectionStringBuilder.InitialCatalog;
        if (string.IsNullOrWhiteSpace(databaseName))
        {
            throw new InvalidOperationException("The background job connection string must specify a database name.");
        }

        connectionStringBuilder.InitialCatalog = "master";
        await using var connection = new SqlConnection(connectionStringBuilder.ConnectionString);
        await connection.OpenAsync();

        await using var existsCommand = connection.CreateCommand();
        existsCommand.CommandText = "SELECT COUNT(1) FROM sys.databases WHERE name = @databaseName";
        _ = existsCommand.Parameters.AddWithValue("@databaseName", databaseName);

        if (Convert.ToInt32(await existsCommand.ExecuteScalarAsync()) > 0)
        {
            return;
        }

        await using var createCommand = connection.CreateCommand();
        createCommand.CommandText = $"CREATE DATABASE {QuoteIdentifier(databaseName)}";
        _ = await createCommand.ExecuteNonQueryAsync();
    }

    private static string QuoteIdentifier(string identifier)
    {
        return $"[{identifier.Replace("]", "]]", StringComparison.Ordinal)}]";
    }

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

        _ = services.AddScoped<ILogEmailDbService, LogEmailDbService>();
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

        #region Scheduler

        _ = services.AddScoped<IProjectScheduler, ProjectScheduler>();

        #endregion

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

        if (app.Logger.IsEnabled(LogLevel.Information))
        {
            app.Logger.LogInformation("This web application is configured with Background Job at: {Endpoint}.", backgroundJobOptions.DashboardUrl);
        }

        return app;
    }
}
