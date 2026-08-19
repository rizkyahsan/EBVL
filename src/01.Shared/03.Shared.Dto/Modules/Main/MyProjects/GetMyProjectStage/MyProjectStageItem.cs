namespace EBVL.Shared.Dto.Modules.Main.MyProjects.GetMyProjectStage;

public sealed class MyProjectStageItem
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

    public required bool IsPicLender { get; set; }
    public required bool IsAllowUpdate { get; set; }

    public required List<MyProjectAttachmentItem> ProjectAttachments { get; set; } = [];
    public required MyProjectLenderReqItem ProjectLenderReq { get; set; }

    public required IEnumerable<AuditItem> Audits { get; init; }
}
