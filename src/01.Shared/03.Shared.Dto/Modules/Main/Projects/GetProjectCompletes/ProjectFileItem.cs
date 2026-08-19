namespace EBVL.Shared.Dto.Modules.Main.Projects.GetProjectCompletes;

public sealed record ProjectFileItem
{
    public required Guid Id { get; init; }

    public required Guid FileStorageId { get; set; }
    public required string FileStorageName { get; set; }
}
