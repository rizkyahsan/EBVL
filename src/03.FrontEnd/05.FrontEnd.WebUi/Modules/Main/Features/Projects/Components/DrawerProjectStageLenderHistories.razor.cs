using EBVL.Shared.Dto.Modules.Main.Projects.GetProjectStage;

namespace EBVL.FrontEnd.WebUi.Modules.Main.Features.Projects.Components;

public partial class DrawerProjectStageLenderHistories
{
    [Parameter, EditorRequired]
    public required List<string> LenderEmails { get; set; } = [];

    [Parameter, EditorRequired]
    public required List<ProjectLenderHistoryItem> ProjectLenderHistories { get; init; }

    private IEnumerable<ProjectLenderHistoryItem> OrderedHistories =>
        ProjectLenderHistories.OrderByDescending(x => x.Created);

    private bool IsLenderUser(ProjectLenderHistoryItem history)
    {
        return LenderEmails.Contains(history.CreatedBy, StringComparer.OrdinalIgnoreCase);
    }
}
