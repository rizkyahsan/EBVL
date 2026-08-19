using EBVL.FrontEnd.Logics.Modules.Main.MyProjects.GetMyProjects;
using EBVL.Shared.Dto.Modules.Main.MyProjects.GetMyProjects;
using Microsoft.JSInterop;

namespace EBVL.FrontEnd.WebUi.Modules.Main.Features.MyProjects.Pages;

public partial class Index
{
    [Inject]
    public required IJSRuntime JSRuntime { get; init; }

    private List<MyProjectItem> _items = [];
    private readonly HashSet<Guid> _expanded = [];

    protected override async Task OnInitializedAsync()
    {
        _pageTitle = $"{ProjectsDisplayTextFor.MyProject}";

        LoadBreadcrumbs();
        await LoadItem();
    }

    protected override void LoadBreadcrumbs()
    {
        _breadcrumbItems =
        [
            MainPageMyProjectsBreadcrumbFor.Index,
            CommonBreadcrumbFor.Active(_pageTitle)
        ];
    }

    private async Task LoadItem()
    {
        try
        {
            _isLoading = true;

            ClearException();

            var query = new GetMyProjectsQuery();
            var response = await Sender.Send(query);

            _items = [.. response.Items];

            _breadcrumbItems.RemoveAt(_breadcrumbItems.Count - 1);
            _breadcrumbItems.Add(CommonBreadcrumbFor.Active(_pageTitle));
        }
        catch (Exception exception)
        {
            _exception = exception;
            Snackbar.AddErrors(_exception.GetAllErrorMessages());
        }
        finally
        {
            _isLoading = false;
        }
    }

    private bool IsExpanded(Guid projectId)
    {
        return _expanded.Contains(projectId);
    }

    private int GetRowNumber(MyProjectItem item)
    {
        return _items.IndexOf(item) + 1;
    }

    private static bool ShouldShowProjectLenderStatus(MyProjectItem item)
    {
        return item.StatusCode == StatusesCodeFor.ProjectComplete
            || item.ProjectLenderStatusCode == StatusesCodeFor.ProjectLenderLose;
    }

    private static string GetProjectStatusCode(MyProjectItem item)
    {
        return ShouldShowProjectLenderStatus(item)
            ? item.ProjectLenderStatusCode
            : item.StatusCode;
    }

    private static string GetProjectStatusName(MyProjectItem item)
    {
        return ShouldShowProjectLenderStatus(item)
            ? item.ProjectLenderStatusName
            : item.StatusName;
    }

    private void Toggle(MyProjectItem item)
    {
        if (_expanded.Contains(item.Id))
        {
            _ = _expanded.Remove(item.Id);
        }
        else
        {
            _ = _expanded.Add(item.Id);
        }
    }

    private static bool CanUpdate(MyProjectStageItem item)
    {
        return item.IsPicLender
            && item.IsAllowUpdate
            && item.StatusCode == StatusesCodeFor.ProjectStageOnProgress
            && (item.StatusProjectLenderReqCode == StatusesCodeFor.ProjectLenderReqOnProgress
            || item.StatusProjectLenderReqCode == StatusesCodeFor.ProjectLenderReqRevision);
    }

    private void Details(MyProjectItem item)
    {
        NavigationManager.NavigateTo(MainPageMyProjectsRouteFor.Details(item.Id));
    }

    private void StageDetails(MyProjectStageItem item)
    {
        NavigationManager.NavigateTo(MainPageMyProjectsRouteFor.StageDetails(item.Id));
    }

    private void StageUpdate(MyProjectStageItem item)
    {
        NavigationManager.NavigateTo(MainPageMyProjectsRouteFor.StageUpdate(item.Id));
    }
}
