using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Diagnostics;
using EBVL.BackEnd.Infrastructure.Database.Interceptors;

namespace EBVL.BackEnd.Infrastructure.Database;

public static class ConfigureDatabase
{
    public static IServiceCollection AddDatabaseService(this IServiceCollection services, string connectionString, IHealthChecksBuilder healthChecksBuilder)
    {
        _ = services.AddDbContext<IDatabaseService, DatabaseService>(options =>
        {
            _ = options.UseSqlServer(connectionString, builder =>
            {
                _ = builder.MigrationsAssembly(typeof(DatabaseService).Assembly.FullName);
                _ = builder.MigrationsHistoryTable("__EFMigrationsHistory", nameof(EBVL));
                _ = builder.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
            });

            _ = options.ConfigureWarnings(wcb => wcb.Ignore(CoreEventId.RowLimitingOperationWithoutOrderByWarning));
            _ = options.ConfigureWarnings(wcb => wcb.Throw(RelationalEventId.MultipleCollectionIncludeWarning));
        });

        var builder = new SqlConnectionStringBuilder(connectionString);
        var databaseName = builder.InitialCatalog;

        _ = healthChecksBuilder.AddSqlServer(
            connectionString: connectionString,
            name: $"Application Database: SQL Server ({databaseName})",
            tags: ["Database"]);

        _ = services.AddScoped<AuditingSaveChangesInterceptor>();
        _ = services.AddScoped<DatabaseMigrator>();

        _ = services.AddTransient<AuditSeeder>();
        _ = services.AddTransient<CountrySeeder>();

        return services;
    }

    public static async Task InitializeDatabase(this IHost host, bool isDataSeedingEnabled)
    {
        using var serviceScope = host.Services.CreateScope();
        var serviceProvider = serviceScope.ServiceProvider;

        var databaseMigrator = serviceProvider.GetRequiredService<DatabaseMigrator>();
        await databaseMigrator.Migrate();

        var auditSeeder = serviceProvider.GetRequiredService<AuditSeeder>();
        await auditSeeder.RecordSystemStartup();

        if (isDataSeedingEnabled)
        {
            var countrySeeder = serviceProvider.GetRequiredService<CountrySeeder>();
            await countrySeeder.SeedCountries();
        }
    }
}
