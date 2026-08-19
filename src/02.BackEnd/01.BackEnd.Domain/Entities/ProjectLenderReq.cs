namespace EBVL.BackEnd.Domain.Entities;

public sealed class ProjectLenderReq : ModifiableEntity
{
    public required Guid ProjectId { get; set; }
    public required Guid ProjectLenderId { get; set; }
    public required Guid ProjectStageId { get; set; }

    public required Guid StatusId { get; set; }

    public Project Project { get; set; } = default!;
    public ProjectLender ProjectLender { get; set; } = default!;
    public ProjectStage ProjectStage { get; set; } = default!;

    public ICollection<ProjectLenderReqFile> ProjectLenderReqFiles { get; set; } = new HashSet<ProjectLenderReqFile>();
    public ICollection<ProjectLenderHistory> ProjectLenderHistories { get; set; } = new HashSet<ProjectLenderHistory>();
}
