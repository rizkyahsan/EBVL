namespace EBVL.BackEnd.Infrastructure.LocalIdentity.Migrator;

public sealed class LocalIdentityMigrator(ILogger<LocalIdentityMigrator> logger, AspNetLocalIdentityDatabase aspNetLocalIdentityDatabase)
{
    public async Task MigrateAsync()
    {
        var pendingMigrations = await aspNetLocalIdentityDatabase.Database.GetPendingMigrationsAsync();

        if (pendingMigrations.Any())
        {
            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation("Applying identity database migration...");
            }

            await aspNetLocalIdentityDatabase.Database.MigrateAsync();
        }
        else
        {
            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation("Identity database is up to date. No identity database migration required.");
            }
        }
    }
}
