namespace EBVL.BackEnd.Domain.Entities;

public sealed class FileStorage : FileEntity
{
    public required string FileExtension { get; set; }
    public required FileStorageType StorageType { get; set; }

    // 🔑 Blob-specific fields
    public string? BlobName { get; set; }          // Unique blob identifier
    public string? ContainerName { get; set; }     // Which container it lives in
    public string? SecureUrl { get; set; }         // Optional: last generated SAS URL

    [ExcludeFromAudit]
    public byte[]? FileData { get; set; }
    [ExcludeFromAudit]
    public string? FileHash { get; set; }
}
