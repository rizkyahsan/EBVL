namespace EBVL.Shared.Dto.Modules.Main.Projects.GetLastProjectStage;

public sealed record ProjectReqItem
{
    public required Guid Id { get; init; }

    public required string Name { get; set; }
    public required string Desc { get; set; }
    public required int SortNo { get; set; }
    public required bool IsRequired { get; set; }
}
