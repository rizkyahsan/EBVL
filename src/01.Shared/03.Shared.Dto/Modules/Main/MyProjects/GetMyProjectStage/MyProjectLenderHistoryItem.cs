namespace EBVL.Shared.Dto.Modules.Main.MyProjects.GetMyProjectStage;

public sealed record MyProjectLenderHistoryItem
{
    public required Guid Id { get; init; }

    public string Remarks { get; set; } = string.Empty;
    public DateTimeOffset Created { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
}
