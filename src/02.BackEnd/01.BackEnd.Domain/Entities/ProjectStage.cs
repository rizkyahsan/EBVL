namespace EBVL.BackEnd.Domain.Entities;

public sealed class ProjectStage : ModifiableEntity
{
    public required Guid ProjectId { get; set; }

    public required int Level { get; set; }
    public required string Name { get; set; }
    public required string Desc { get; set; }
    public required DateTimeOffset DueDate { get; set; }

    public required Guid StatusId { get; set; }

    public Project Project { get; set; } = default!;

    public ICollection<ProjectAttachment> ProjectAttachments { get; set; } = new HashSet<ProjectAttachment>();
    public ICollection<ProjectReq> ProjectReqs { get; set; } = new HashSet<ProjectReq>();
    public ICollection<ProjectLenderReq> ProjectLenderReqs { get; set; } = new HashSet<ProjectLenderReq>();
}
