namespace EBVL.Shared.Dto.Common.Audits;

public abstract record AuditItemBase
{
    public Guid Id { get; init; }
    public DateTimeOffset Created { get; init; }
    public string CreatedBy { get; init; } = default!;

    public Guid EntityId { get; init; }
    public string EntityName { get; init; } = default!;
    public AuditActionType ActionType { get; init; }
    public string ActionName { get; init; } = default!;
    public IEnumerable<AuditItemProperty> Properties { get; init; } = [];
}
