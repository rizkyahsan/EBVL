namespace EBVL.BackEnd.Domain.Entities;

public sealed class ProjectReq : ModifiableEntity
{
    public required Guid ProjectId { get; set; }
    public required Guid ProjectStageId { get; set; }

    public required string Name { get; set; }
    public required string Desc { get; set; }
    public required int SortNo { get; set; }
    public required bool IsRequired { get; set; }

    public Project Project { get; set; } = default!;
    public ProjectStage ProjectStage { get; set; } = default!;

    public ICollection<ProjectLenderReqFile> ProjectLenderReqFiles { get; set; } = new HashSet<ProjectLenderReqFile>();
}
