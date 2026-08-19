namespace EBVL.BackEnd.Domain.Entities;

public sealed class ProjectLenderReqFile : ModifiableEntity
{
    public required Guid ProjectId { get; set; }
    public required Guid ProjectReqId { get; set; }
    public required Guid ProjectLenderReqId { get; set; }

    public required Guid FileStorageId { get; set; }

    public Project Project { get; set; } = default!;
    public ProjectReq ProjectReq { get; set; } = default!;
    public ProjectLenderReq ProjectLenderReq { get; set; } = default!;
}
