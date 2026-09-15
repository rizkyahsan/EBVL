using EBVL.Shared.Statics.VendorRegistrations;

namespace EBVL.BackEnd.Infrastructure.Database.Seeders;

public sealed class DocumentDefinitionSeeder(IDatabaseService db)
{
    public async Task SeedVendorRegistrationDocuments()
    {
        for (var index = 0; index < DocumentEvidenceFor.All.Count; index++)
        {
            var legacy = DocumentEvidenceFor.All[index];
            var existing = await db.DocumentDefinitions.SingleOrDefaultAsync(x => !x.IsDeleted && x.BusinessProcess == "Vendor Registration" && (x.Code == legacy.Key || x.Name == legacy.Name));
            if (existing is null)
            {
                _ = await db.DocumentDefinitions.AddAsync(new DocumentDefinition
                {
                    Code = legacy.Key,
                    BusinessProcess = "Vendor Registration",
                    Name = legacy.Name,
                    Order = index + 1,
                    MaxSizeMb = 50,
                    IsMandatory = true,
                    IsActive = true
                });
            }
            else if (string.IsNullOrWhiteSpace(existing.Code))
            {
                existing.Code = legacy.Key;
                existing.Order = index + 1;
            }
        }

        _ = await db.SaveAsync(nameof(SeedVendorRegistrationDocuments));
    }
}
