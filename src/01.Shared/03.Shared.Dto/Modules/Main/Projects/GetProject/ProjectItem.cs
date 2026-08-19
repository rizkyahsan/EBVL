namespace EBVL.Shared.Dto.Modules.Main.Projects.GetProject;

public sealed record ProjectItem
{
    public required Guid Id { get; init; }

    public required string Title { get; set; }
    public required string Desc { get; set; }
    public required string Objective { get; set; }
    public required string FinanceType { get; set; }

    public required Guid StatusId { get; set; }
    public required string StatusCode { get; set; }
    public required string StatusName { get; set; }

    public required List<ProjectStageItem> ProjectStages { get; set; } = [];
    public required List<ProjectLenderItem> ProjectLenders { get; set; } = [];

    public required IEnumerable<AuditItem> Audits { get; init; }
}
