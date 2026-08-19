namespace EBVL.BackEnd.Infrastructure.Database.Seeders;

public sealed class StatusSeeder(IDatabaseService databaseService)
{
    public async Task SeedStatuses()
    {
        foreach (var status in InitialStatuses.All)
        {
            if (!await databaseService.Statuses.AnyAsync(x => x.Id == status.Id))
            {
                _ = await databaseService.Statuses.AddAsync(status);
            }
        }

        _ = await databaseService.SaveAsync(nameof(SeedStatuses));
    }
}
