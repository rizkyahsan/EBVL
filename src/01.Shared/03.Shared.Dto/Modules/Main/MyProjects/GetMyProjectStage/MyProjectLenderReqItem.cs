namespace EBVL.Shared.Dto.Modules.Main.MyProjects.GetMyProjectStage;

public sealed record MyProjectLenderReqItem
{
    public required Guid Id { get; init; }

    public required Guid ProjectLenderId { get; set; }
    public required string ProjectLenderName { get; set; }

    public required Guid StatusId { get; set; }
    public required string StatusCode { get; set; }
    public required string StatusName { get; set; }

    public List<MyProjectReqItem> ProjectReqItems { get; set; } = [];
    public List<MyProjectLenderHistoryItem> ProjectLenderHistories { get; set; } = [];
}
