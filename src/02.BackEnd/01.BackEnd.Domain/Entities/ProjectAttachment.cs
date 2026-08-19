namespace EBVL.BackEnd.Domain.Entities;

public sealed class ProjectAttachment : ModifiableEntity
{
    public required Guid ProjectId { get; set; }
    public required Guid ProjectStageId { get; set; }

    public required string Name { get; set; }
    public required string Desc { get; set; }
    public required int SortNo { get; set; }
    public required Guid FileStorageId { get; set; }

    public Project Project { get; set; } = default!;
    public ProjectStage ProjectStage { get; set; } = default!;
}
