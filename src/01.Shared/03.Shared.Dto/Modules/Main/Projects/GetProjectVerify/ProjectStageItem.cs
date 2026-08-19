namespace EBVL.Shared.Dto.Modules.Main.Projects.GetProjectVerify;

public sealed class ProjectStageItem
{
    public required Guid Id { get; init; }

    public required Guid ProjectId { get; set; }
    public required string ProjectTitle { get; set; }
    public required string ProjectDesc { get; set; }
    public required string ProjectObjective { get; set; }
    public required string ProjectFinanceType { get; set; }
    public required Guid ProjectStatusId { get; set; }
    public required string ProjectStatusCode { get; set; }
    public required string ProjectStatusName { get; set; }

    public required int Level { get; set; }
    public required string Name { get; set; }
    public required string Desc { get; set; }
    public required DateTime? DueDate { get; set; }

    public required Guid StatusId { get; set; }
    public required string StatusCode { get; set; }
    public required string StatusName { get; set; }

    public required List<ProjectLenderReqItem> ProjectLenderReqs { get; set; } = [];

    public required IEnumerable<AuditItem> Audits { get; init; }
}
