namespace EBVL.BackEnd.Infrastructure.Database.Seeders;

public sealed class CountrySeeder(
    IDatabaseService databaseService,
    ILogger<CountrySeeder> logger)
{
    public async Task SeedCountries()
    {
        foreach (var country in InitialCountries.All)
        {
            if (!await databaseService.Countries.AnyAsync(x => x.Id == country.Id))
            {
                logger.LogInformation("Seeding data {EntityType} {EntityName}...",
                    nameof(Country), country.Name);

                _ = await databaseService.Countries.AddAsync(country);
            }
        }

        _ = await databaseService.SaveAsync(nameof(SeedCountries));
    }
}
