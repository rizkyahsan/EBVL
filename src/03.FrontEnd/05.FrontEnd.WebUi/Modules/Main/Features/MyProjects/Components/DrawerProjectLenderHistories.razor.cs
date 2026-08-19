using EBVL.Shared.Dto.Modules.Main.MyProjects.GetMyProjectStage;
using Pertamina.Services.CurrentUser;

namespace EBVL.FrontEnd.WebUi.Modules.Main.Features.MyProjects.Components;

public partial class DrawerProjectLenderHistories
{
    [Inject]
    private ICurrentUserService CurrentUserService { get; set; } = default!;

    [Parameter, EditorRequired]
    public required List<MyProjectLenderHistoryItem> ProjectLenderHistories { get; init; }

    private IEnumerable<MyProjectLenderHistoryItem> OrderedHistories =>
        ProjectLenderHistories.OrderByDescending(x => x.Created);

    private bool IsCurrentUser(MyProjectLenderHistoryItem history)
    {
        return string.Equals(history.CreatedBy, CurrentUserService.Username, StringComparison.OrdinalIgnoreCase);
    }
}
