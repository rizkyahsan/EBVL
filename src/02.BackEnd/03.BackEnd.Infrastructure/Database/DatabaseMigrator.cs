namespace EBVL.BackEnd.Infrastructure.Database;

public sealed class DatabaseMigrator(ILogger<DatabaseMigrator> logger, DatabaseService databaseService)
{
    public async Task Migrate()
    {
        var pendingMigrations = await databaseService.Database.GetPendingMigrationsAsync();

        if (pendingMigrations.Any())
        {
            logger.LogInformation("Applying database migration...");

            await databaseService.Database.MigrateAsync();
        }
        else
        {
            logger.LogInformation("Database is up to date. No database migration required.");
        }
    }
}
