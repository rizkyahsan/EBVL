namespace EBVL.Shared.Dto.Modules.MasterData.EmailTemplates.GetEmailTemplates;

public sealed record EmailTemplateItem
{
    public required Guid Id { get; init; }
    public required string Module { get; init; }
    public required string Action { get; init; }
    public string DefaultTo { get; init; } = string.Empty;
    public string DefaultCc { get; init; } = string.Empty;
    public required string Subject { get; init; }
    public required string Content { get; init; }
}
