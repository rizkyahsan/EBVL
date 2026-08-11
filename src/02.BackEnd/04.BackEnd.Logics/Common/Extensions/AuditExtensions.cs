using Pertamina.Common.Domain.Interfaces;
using Pertamina.Extensions.Json;
using EBVL.Shared.Dto.Common.Audits;

namespace EBVL.BackEnd.Logics.Common.Extensions;

public static class AuditExtensions
{
    public static IList<T> ToAuditItems<T>(this IEnumerable<Audit> audits)
        where T : AuditItemBase, new()
    {
        return [.. audits.Select(audit => audit.ToAuditItem<T>())];
    }

    public static T ToAuditItem<T>(this Audit audit)
        where T : AuditItemBase, new()
    {
        return new T
        {
            Id = audit.Id,
            Created = audit.Created,
            CreatedBy = audit.CreatedBy,
            EntityId = audit.EntityId,
            EntityName = audit.EntityName,
            ActionType = audit.ActionType,
            ActionName = audit.GetActionName(),
            Properties = audit.GetProperties()
        };
    }

    public static string GetActionName(this Audit audit)
    {
        return audit.ActionName
            .Replace("Command", string.Empty)
            .Replace("Query", string.Empty)
            .AddSpacesToSentence(true);
    }

    public static IList<AuditItemProperty> GetProperties(this Audit audit)
    {
        var differences = new List<AuditItemProperty>();

        var oldProperties = audit.OldValues.ToProperties();
        var newProperties = audit.NewValues.ToProperties();

        foreach (var property in oldProperties)
        {
            var propertyName = property.Key
                .AddSpacesToSentence(true)
                .Replace(nameof(CommonDisplayTextFor.Id), CommonDisplayTextFor.Id);

            differences.Add(new AuditItemProperty
            {
                Name = propertyName,
                OldValue = property.Value,
                NewValue = newProperties.FirstOrDefault(x => x.Key == property.Key).Value
            });
        }

        foreach (var property in newProperties)
        {
            var propertyName = property.Key
                .AddSpacesToSentence(true)
                .Replace(nameof(CommonDisplayTextFor.Id), CommonDisplayTextFor.Id);

            if (differences.Any(x => x.Name == propertyName) ||
                (audit.ActionType is AuditActionType.Edit && (property.Key is nameof(IModifiable.Modified) or nameof(IModifiable.ModifiedBy))))
            {
                continue;
            }

            differences.Add(new AuditItemProperty
            {
                Name = propertyName,
                OldValue = string.Empty,
                NewValue = property.Value
            });
        }

        return differences;
    }
}
