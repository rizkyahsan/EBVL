namespace EBVL.Shared.Dto.Modules.Administration.Audits.GetAudits;

public sealed record AuditItem
{
    public required Guid Id { get; init; }
    public required DateTimeOffset Created { get; init; }
    public required string CreatedBy { get; init; }
    public required AuditActionType ActionType { get; init; }
    public required string ActionName { get; init; }
    public required Guid EntityId { get; init; }
    public required string EntityName { get; init; }
}
