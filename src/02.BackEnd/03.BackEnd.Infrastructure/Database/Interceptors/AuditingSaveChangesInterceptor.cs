using System.Text.Json;
using EBVL.Shared.Enums;
using EBVL.Shared.Statics.Common;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Pertamina.Common.Domain.Interfaces;
using Pertamina.Services.CurrentUser;
using Pertamina.Services.DateAndTime;

namespace EBVL.BackEnd.Infrastructure.Database.Interceptors;

public sealed class AuditingSaveChangesInterceptor(
    IDateAndTimeService dateAndTimeService,
    ICurrentUserService currentUserService)
    : SaveChangesInterceptor
{
    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        var context = eventData.Context!;
        var now = TimeZoneInfo.ConvertTime(dateAndTimeService.Now, TimezoneFor.WibTimeZone);
        var username = currentUserService.Username ?? "system@pertamina.com";

        foreach (var entry in context.ChangeTracker.Entries<IHasKey>())
        {
            if (entry.State is EntityState.Added && entry.Entity is ICreatable creatable)
            {
                creatable.Created = now;
                creatable.CreatedBy = username;
            }
            else if (entry.State is EntityState.Modified && entry.Entity is IModifiable modifiable)
            {
                modifiable.Modified = now;
                modifiable.ModifiedBy = username;
            }
        }

        if (context is DatabaseService databaseService)
        {
            var actionName = databaseService.CurrentActionName;

            if (string.IsNullOrWhiteSpace(actionName))
            {
                actionName = "Unknown";
            }

            var audits = new List<Audit>();

            foreach (var entry in context.ChangeTracker.Entries())
            {
                if (entry.Entity is Audit)
                {
                    continue;
                }

                if (entry.IsExcludedFromAudit())
                {
                    continue;
                }

                AuditActionType? actionType = entry.State switch
                {
                    EntityState.Added => AuditActionType.Add,
                    EntityState.Modified => AuditActionType.Edit,
                    EntityState.Deleted => AuditActionType.Delete,
                    EntityState.Detached => null,
                    EntityState.Unchanged => null,
                    _ => null
                };

                if (actionType is null)
                {
                    continue;
                }

                string? oldValues = null;
                var newValues = string.Empty;

                if (entry.State is EntityState.Added)
                {
                    var currentValues = entry.CurrentValues.Properties
                        .Where(property => property.Name
                            is not (nameof(IModifiable.Modified))
                            and not (nameof(IModifiable.ModifiedBy))
                            and not (nameof(ICreatable.IsDeleted))
                            && !entry.Property(property.Name).IsExcludedFromAudit())
                        .ToDictionary(property => property.Name, property => entry.CurrentValues[property.Name]);

                    newValues = JsonSerializer.Serialize(currentValues);
                }
                else if (entry.State is EntityState.Modified)
                {
                    var originalValues = entry.OriginalValues.Properties
                        .Where(property => entry.Property(property.Name).IsModified && !entry.Property(property.Name).IsExcludedFromAudit())
                        .ToDictionary(property => property.Name, property => entry.OriginalValues[property.Name]);

                    oldValues = JsonSerializer.Serialize(originalValues);

                    var currentValues = entry.CurrentValues.Properties
                        .Where(property => entry.Property(property.Name).IsModified && !entry.Property(property.Name).IsExcludedFromAudit())
                        .ToDictionary(property => property.Name, property => entry.CurrentValues[property.Name]);

                    newValues = JsonSerializer.Serialize(currentValues);
                }
                else if (entry.State is EntityState.Deleted)
                {
                    var originalValues = entry.OriginalValues.Properties
                        .Where(property => !entry.Property(property.Name).IsExcludedFromAudit())
                        .ToDictionary(property => property.Name, property => entry.OriginalValues[property.Name]);

                    oldValues = JsonSerializer.Serialize(originalValues);
                }

                var audit = new Audit
                {
                    Created = now,
                    CreatedBy = username,
                    ActionType = actionType.Value,
                    ActionName = actionName,
                    EntityId = (Guid)entry.Property(nameof(IHasKey.Id)).CurrentValue!,
                    EntityName = entry.Entity.GetType().Name,
                    OldValues = oldValues,
                    NewValues = newValues
                };

                audits.Add(audit);
            }

            if (audits.Count != 0)
            {
                await databaseService.Audits.AddRangeAsync(audits, cancellationToken);
            }
        }

        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}
