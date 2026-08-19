namespace EBVL.Shared.Dto.Modules.Main.MyProjects.GetMyProjectStage;

public sealed record MyProjectReqItem
{
    public required Guid Id { get; init; }

    public required string Name { get; set; }
    public required string Desc { get; set; }
    public required int SortNo { get; set; }
    public required bool IsRequired { get; set; }

    public List<MyProjectLenderReqFileItem> ProjectLenderReqFiles { get; set; } = [];
}
