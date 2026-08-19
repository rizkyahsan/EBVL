namespace EBVL.BackEnd.Infrastructure.Database.Seeders;

public sealed class EmailTemplateSeeder(IDatabaseService databaseService)
{
    public async Task SeederEmailTemplate()
    {
        foreach (var emailTemplate in InitialEmailTemplate.All)
        {
            if (!await databaseService.EmailTemplates.AnyAsync(x => x.Module == emailTemplate.Module && x.Action == emailTemplate.Action))
            {
                _ = await databaseService.EmailTemplates.AddAsync(emailTemplate);
            }
        }

        _ = await databaseService.SaveAsync(nameof(SeederEmailTemplate));
    }
}
