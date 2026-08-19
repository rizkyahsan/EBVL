namespace EBVL.Shared.Dto.Modules.Main.Projects.GetProject;

public sealed class ProjectStageItem
{
    public required Guid Id { get; init; }

    public required int Level { get; set; }
    public required string Name { get; set; }
    public required string Desc { get; set; }
    public required DateTime? DueDate { get; set; }

    public required Guid StatusId { get; set; }
    public required string StatusCode { get; set; }
    public required string StatusName { get; set; }
}
