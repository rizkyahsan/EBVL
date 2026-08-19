namespace EBVL.Shared.Dto.Modules.Main.MyProjects.GetMyProjectStage;

public sealed record MyProjectLenderReqFileItem
{
    public required Guid Id { get; init; }

    public required Guid FileStorageId { get; set; }
    public required string FileStorageName { get; set; }
}
