using EBVL.Shared.Statics.VendorRegistrations;
using EBVL.Shared.Enums;

namespace EBVL.BackEnd.Infrastructure.Database.Seeders;

public sealed class DocumentDefinitionSeeder(IDatabaseService db)
{
    public async Task SeedVendorRegistrationDocuments()
    {
        var set = await db.DocumentRequirementSets.Include(x => x.Requirements).SingleOrDefaultAsync(x => !x.IsDeleted && x.BusinessProcess == "Vendor Registration" && x.Status == QuestionnaireStatus.Publish);
        if (set is null)
        {
            var id = Guid.CreateVersion7();
            set = new DocumentRequirementSet { Id = id, DocumentRequirementSetSeriesId = id, BusinessProcess = "Vendor Registration", Version = 1, Status = QuestionnaireStatus.Publish, PublishedAt = DateTimeOffset.UtcNow, PublishedBy = "EBVLSystem" };
            _ = await db.DocumentRequirementSets.AddAsync(set);
        }

        for (var index = 0; index < DocumentEvidenceFor.All.Count; index++)
        {
            var legacy = DocumentEvidenceFor.All[index];
            var existing = set.Requirements.SingleOrDefault(x => !x.IsDeleted && (x.Code == legacy.Key || x.Name == legacy.Name));
            if (existing is null)
            {
                _ = await db.DocumentDefinitions.AddAsync(new DocumentDefinition
                {
                    Code = legacy.Key,
                    DocumentRequirementSetId = set.Id,
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
