namespace EBVL.Shared.Dto.Modules.Main.MyProjects.GetMyProject;

public sealed class MyProjectStageItem
{
    public required Guid Id { get; init; }

    public required int Level { get; set; }
    public required string Name { get; set; }
    public required string Desc { get; set; }
    public required DateTime? DueDate { get; set; }

    public required Guid StatusId { get; set; }
    public required string StatusCode { get; set; }
    public required string StatusName { get; set; }

    public required bool IsPicLender { get; set; }
    public required bool IsAllowUpdate { get; set; }

    public required Guid StatusProjectLenderReqId { get; set; }
    public required string StatusProjectLenderReqCode { get; set; }
    public required string StatusProjectLenderReqName { get; set; }
}
