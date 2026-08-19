namespace EBVL.Shared.Dto.Modules.Main.Projects.GetProjectStage;

public sealed record ProjectLenderReqItem
{
    public required Guid Id { get; init; }

    public required Guid ProjectLenderId { get; set; }
    public required string ProjectLenderName { get; set; }
    public required List<string> ProjectLenderEmails { get; set; }

    public required Guid StatusId { get; set; }
    public required string StatusCode { get; set; }
    public required string StatusName { get; set; }

    public List<ProjectReqItem> ProjectReqItems { get; set; } = [];
    public List<ProjectLenderHistoryItem> ProjectLenderHistories { get; set; } = [];
}

