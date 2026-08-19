namespace EBVL.Shared.Dto.Modules.MasterData.EmailTemplates.GetEmailTemplate;

public sealed record EmailTemplateItem
{
    public required Guid Id { get; init; }
    public required DateTimeOffset Created { get; init; }
    public required string CreatedBy { get; init; }
    public required DateTimeOffset? Modified { get; init; }
    public required string? ModifiedBy { get; init; }

    public required string Module { get; init; }
    public required string Action { get; init; }
    public string To { get; init; } = string.Empty;
    public string Cc { get; init; } = string.Empty;
    public required string Subject { get; init; }
    public required string Content { get; init; }

    public required IEnumerable<AuditItem> Audits { get; init; }
}
