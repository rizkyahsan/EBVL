namespace EBVL.Shared.Dto.Modules.Main.Projects.GetProjectVerify;

public sealed record ProjectLenderHistoryItem
{
    public required Guid Id { get; init; }

    public string Remarks { get; set; } = string.Empty;
    public DateTimeOffset Created { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
}

