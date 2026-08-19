namespace EBVL.BackEnd.Domain.Entities;

public sealed class ProjectLenderHistory : ModifiableEntity
{
    public required Guid ProjectId { get; set; }
    public required Guid ProjectLenderReqId { get; set; }

    public string Remarks { get; set; } = string.Empty;

    public Project Project { get; set; } = default!;
    public ProjectLenderReq ProjectLenderReq { get; set; } = default!;
}
