namespace EBVL.Shared.Dto.Modules.Main.VendorRegistrations.DocumentEvidence;

public sealed record DocumentEvidenceItemRequest
{
    public required string Key { get; init; }
    public string? FileName { get; set; }
}
