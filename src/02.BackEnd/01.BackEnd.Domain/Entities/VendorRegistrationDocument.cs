namespace EBVL.BackEnd.Domain.Entities;

public sealed class VendorRegistrationDocument : ModifiableEntity
{
    public required Guid VendorRegistrationId { get; set; }
    public required Guid DocumentTemplateId { get; set; }
    public required string OriginalFileName { get; set; }
    public required string StoredFileName { get; set; }
    public required string FileContentType { get; set; }
    public required long FileSize { get; set; }
    public string? StorageFileId { get; set; }
    public string? ContentHash { get; set; }
    public DateTimeOffset? ValidUntil { get; set; }
    public bool IsVerified { get; set; }
    public VendorRegistration VendorRegistration { get; set; } = default!;
    public DocumentTemplate DocumentTemplate { get; set; } = default!;
}
