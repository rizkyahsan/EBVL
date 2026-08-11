namespace EBVL.Shared.Dto.Modules.Examples.Documents.GetDocument;

public sealed record DocumentItem
{
    public required Guid Id { get; init; }
    public required DateTimeOffset Created { get; init; }
    public required string CreatedBy { get; init; }
    public required DateTimeOffset? Modified { get; init; }
    public required string? ModifiedBy { get; init; }

    public required string Description { get; init; }
    public required string FileName { get; init; }
    public required long FileSize { get; init; }

    public required IEnumerable<AuditItem> Audits { get; init; }
}
