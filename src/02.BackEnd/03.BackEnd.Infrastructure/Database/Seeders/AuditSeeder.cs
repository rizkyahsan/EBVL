using System.Text.Json;
using Pertamina.Services.DateAndTime;
using EBVL.Shared.Enums;

namespace EBVL.BackEnd.Infrastructure.Database.Seeders;

public sealed class AuditSeeder(
    IDatabaseService databaseService,
    IDateAndTimeService dateAndTimeService)
{
    public async Task RecordSystemStartup()
    {
        var auditId = Guid.CreateVersion7();

        var newValues = new Dictionary<string, object?>
        {
            {
                "Message",
                $"Started on {dateAndTimeService.Now.LocalDateTime:F}"
            }
        };

        var audit = new Audit
        {
            Id = auditId,
            EntityName = nameof(Audit),
            ActionType = AuditActionType.Add,
            ActionName = nameof(RecordSystemStartup),
            EntityId = auditId,
            OldValues = null,
            NewValues = JsonSerializer.Serialize(newValues),
            Created = dateAndTimeService.Now,
            CreatedBy = InitialValueFor.CreatedBy
        };

        _ = await databaseService.Audits.AddAsync(audit);
        _ = await databaseService.SaveAsync(nameof(AuditSeeder));
    }
}
