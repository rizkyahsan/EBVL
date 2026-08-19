namespace EBVL.Shared.Dto.Modules.Main.Projects.GetProjectStage;

public sealed record ProjectLenderReqFileItem
{
    public required Guid Id { get; init; }

    public required Guid FileStorageId { get; set; }
    public required string FileStorageName { get; set; }
}
