namespace EBVL.BackEnd.Domain.Entities;

public sealed class ProjectLender : ModifiableEntity
{
    public required Guid ProjectId { get; set; }
    public required Guid LenderId { get; set; }

    public string Note { get; set; } = string.Empty;
    public Guid? FileStorageId { get; set; }

    public required Guid StatusId { get; set; }

    public Project Project { get; set; } = default!;
    public Lender Lender { get; set; } = default!;

    public ICollection<ProjectLenderReq> ProjectLenderReqs { get; set; } = new HashSet<ProjectLenderReq>();
}
