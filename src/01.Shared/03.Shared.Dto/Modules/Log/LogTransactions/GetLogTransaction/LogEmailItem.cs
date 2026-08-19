namespace EBVL.Shared.Dto.Modules.Log.LogTransactions.GetLogTransaction;

public sealed record LogTransactionItem
{
    public required Guid Id { get; init; }
    public required DateTimeOffset Created { get; init; }
    public required string CreatedBy { get; init; }
    public required DateTimeOffset? Modified { get; init; }
    public string? ModifiedBy { get; init; }

    public required Guid ProjectId { get; init; }
    public Guid? ProjectStageId { get; init; }
    public Guid? ProjectLenderId { get; init; }
    public required string Action { get; init; }
    public required string Role { get; init; }

    public required IEnumerable<AuditItem> Audits { get; init; }
}
