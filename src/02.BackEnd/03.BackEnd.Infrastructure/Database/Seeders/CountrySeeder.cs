namespace EBVL.BackEnd.Infrastructure.Database.Seeders;

public sealed class CountrySeeder(IDatabaseService databaseService)
{
    public async Task SeedCountries()
    {
        foreach (var country in InitialCountries.All)
        {
            if (!await databaseService.Countries.AnyAsync(x => x.Id == country.Id))
            {
                _ = await databaseService.Countries.AddAsync(country);
            }
        }

        _ = await databaseService.SaveAsync(nameof(SeedCountries));
    }
}
