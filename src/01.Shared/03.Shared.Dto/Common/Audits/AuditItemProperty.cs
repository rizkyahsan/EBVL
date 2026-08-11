namespace EBVL.Shared.Dto.Common.Audits;

public sealed record AuditItemProperty
{
    public required string Name { get; init; }
    public required string OldValue { get; set; }
    public required string NewValue { get; set; }
}
