namespace EBVL.Shared.Dto.Modules.Main.MyProjects.GetMyProjects;

public sealed class MyProjectItem
{
    public required Guid Id { get; init; }

    public required string Title { get; set; }

    public required Guid StatusId { get; set; }
    public required string StatusCode { get; set; }
    public required string StatusName { get; set; }

    public required Guid ProjectLenderId { get; init; }
    public required Guid ProjectLenderStatusId { get; set; }
    public required string ProjectLenderStatusCode { get; set; }
    public required string ProjectLenderStatusName { get; set; }

    public required List<MyProjectStageItem> ProjectStages { get; set; } = [];
}
