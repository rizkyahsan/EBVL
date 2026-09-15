using EBVL.BackEnd.Infrastructure.Database.Interceptors;
using EBVL.BackEnd.Infrastructure.Database.Seeders;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace EBVL.BackEnd.Infrastructure.Database;

public static class ConfigureDatabase
{
    #region Service Registration

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
        _ = services.AddTransient<ConfigurationSeeder>();
        _ = services.AddTransient<CountrySeeder>();
        _ = services.AddTransient<EmailTemplateSeeder>();
        _ = services.AddTransient<QuestionnaireSeeder>();
        _ = services.AddTransient<DocumentDefinitionSeeder>();
        _ = services.AddTransient<DemoExternalUserSeeder>();

        return services;
    }

    #endregion

    #region Database Initialization

    public static async Task InitializeDatabase(this IHost host, bool isDataSeedingEnabled)
    {
        using var serviceScope = host.Services.CreateScope();
        var serviceProvider = serviceScope.ServiceProvider;

        var databaseMigrator = serviceProvider.GetRequiredService<DatabaseMigrator>();
        await databaseMigrator.Migrate();

        var auditSeeder = serviceProvider.GetRequiredService<AuditSeeder>();
        await auditSeeder.RecordSystemStartup();

        var demoExternalUserSeeder = serviceProvider.GetRequiredService<DemoExternalUserSeeder>();
        await demoExternalUserSeeder.SeedDemoExternalUser();

        if (isDataSeedingEnabled)
        {
            var configurationSeeder = serviceProvider.GetRequiredService<ConfigurationSeeder>();
            await configurationSeeder.SeedConfigurations();

            var countrySeeder = serviceProvider.GetRequiredService<CountrySeeder>();
            await countrySeeder.SeedCountries();

            var emailTemplateSeeder = serviceProvider.GetRequiredService<EmailTemplateSeeder>();
            await emailTemplateSeeder.SeederEmailTemplate();

            var questionnaireSeeder = serviceProvider.GetRequiredService<QuestionnaireSeeder>();
            await questionnaireSeeder.SeedQuestionnaires();

            var documentDefinitionSeeder = serviceProvider.GetRequiredService<DocumentDefinitionSeeder>();
            await documentDefinitionSeeder.SeedVendorRegistrationDocuments();
        }
    }

    #endregion
}
