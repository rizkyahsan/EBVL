namespace EBVL.Shared.Statics.Audits;

public static class DisplayTextFor
{
    public const string Audit = nameof(Audit);
    public const string Audits = nameof(Audits);

    public const string AuditId = "Audit ID";
    public const string ActionType = "Action Type";
    public const string ActionName = "Action Name";
    public const string EntityId = "Entity ID";
    public const string EntityName = "Entity Name";

    public const string Property = nameof(Property);
    public const string Properties = nameof(Properties);
    public const string UpdatedProperties = $"Updated {Properties}";
    public const string Value = nameof(Value);
    public const string OldValue = $"Old {Value}";
    public const string OldValues = $"{OldValue}s";
    public const string NewValue = $"New {Value}";
    public const string NewValues = $"{NewValue}s";

    public const string AuditLogs = $"{Audit} Logs";
    public const string UnknownAction = "Unknown Action";
}
