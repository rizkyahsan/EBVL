namespace EBVL.Shared.Dto.Modules.Main.Projects.GetProject;

public sealed record ProjectLenderItem
{
    public required Guid Id { get; init; }

    public required Guid LenderId { get; set; }
    public string LenderName { get; set; } = string.Empty;

    public string Note { get; set; } = string.Empty;
    public Guid? FileStorageId { get; set; }
    public required string FileStorageName { get; set; }

    public required Guid StatusId { get; set; }
    public required string StatusCode { get; set; }
    public required string StatusName { get; set; }
}
