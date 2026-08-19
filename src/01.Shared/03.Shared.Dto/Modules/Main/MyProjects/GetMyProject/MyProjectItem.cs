namespace EBVL.Shared.Dto.Modules.Main.MyProjects.GetMyProject;

public sealed record MyProjectItem
{
    public required Guid Id { get; init; }

    public required string Title { get; set; }
    public required string Desc { get; set; }
    public required string Objective { get; set; }
    public required string FinanceType { get; set; }

    public required Guid StatusId { get; set; }
    public required string StatusCode { get; set; }
    public required string StatusName { get; set; }

    public required Guid ProjectLenderId { get; init; }
    public string ProjectLenderNote { get; set; } = string.Empty;
    public Guid? FileStorageId { get; set; }
    public required Guid ProjectLenderStatusId { get; set; }
    public required string ProjectLenderStatusCode { get; set; }
    public required string ProjectLenderStatusName { get; set; }

    public required List<MyProjectStageItem> ProjectStages { get; set; } = [];

    public required IEnumerable<AuditItem> Audits { get; init; }
}
