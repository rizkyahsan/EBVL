namespace EBVL.Shared.Dto.Modules.Main.Projects.GetLastProjectStage;

public sealed record ProjectAttachmentItem
{
    public required Guid Id { get; init; }

    public required string Name { get; set; }
    public required string Desc { get; set; }
    public required int SortNo { get; set; }

    public required Guid FileStorageId { get; set; }
    public required string FileStorageName { get; set; }
}
