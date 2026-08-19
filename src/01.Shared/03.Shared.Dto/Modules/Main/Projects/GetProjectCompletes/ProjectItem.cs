namespace EBVL.Shared.Dto.Modules.Main.Projects.GetProjectCompletes;

public sealed record ProjectItem
{
    public required Guid Id { get; init; }

    public required string Title { get; set; }

    public required Guid StatusId { get; set; }
    public required string StatusCode { get; set; }
    public required string StatusName { get; set; }

    public required List<ProjectStageItem> ProjectStages { get; set; } = [];
    public required List<ProjectFileItem> ProjectFiles { get; set; } = [];
}
