namespace EBVL.BackEnd.Infrastructure.Database.Seeders;

public sealed class ConfigurationSeeder(IDatabaseService databaseService)
{
    public async Task SeedConfigurations()
    {
        foreach (var configuration in InitialConfigurations.All)
        {
            if (!await databaseService.Configurations.AnyAsync(x => x.Id == configuration.Id))
            {
                _ = await databaseService.Configurations.AddAsync(configuration);
            }
        }

        _ = await databaseService.SaveAsync(nameof(SeedConfigurations));
    }
}
