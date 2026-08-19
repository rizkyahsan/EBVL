namespace EBVL.BackEnd.Domain.Entities;

public sealed class LogTransaction : ModifiableEntity
{
    public required Guid ProjectId { get; set; }
    public Guid? ProjectStageId { get; set; }
    public Guid? ProjectLenderId { get; set; }
    public required string Action { get; set; }
    public required string Role { get; set; }

    public Project Project { get; set; } = default!;
    public ProjectStage? ProjectStage { get; set; }
    public ProjectLender? ProjectLender { get; set; }
}
