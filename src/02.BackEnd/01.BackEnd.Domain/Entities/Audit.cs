namespace EBVL.BackEnd.Domain.Entities;

public sealed class Audit : CreatableEntity
{
    public required AuditActionType ActionType { get; init; }
    public required string ActionName { get; init; }
    public required Guid EntityId { get; init; }
    public required string EntityName { get; init; }
    public required string? OldValues { get; init; }
    public required string NewValues { get; init; }
}
