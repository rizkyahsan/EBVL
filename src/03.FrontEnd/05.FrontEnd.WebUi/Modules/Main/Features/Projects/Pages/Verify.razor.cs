using EBVL.FrontEnd.Logics.Modules.Main.Projects.GetProjectVerifies;
using EBVL.Shared.Dto.Modules.Main.Projects.GetProjectVerifies;
using Microsoft.JSInterop;

namespace EBVL.FrontEnd.WebUi.Modules.Main.Features.Projects.Pages;

public partial class Verify
{
    [Inject]
    public required IJSRuntime JSRuntime { get; init; }

    private List<ProjectItem> _items = [];
    private readonly HashSet<Guid> _expanded = [];
    private Guid _selectedLender = Guid.Empty;

    protected override async Task OnInitializedAsync()
    {
        _pageTitle = CommonDisplayTextFor.Verify;

        LoadBreadcrumbs();
        await LoadItem();
    }

    protected override void LoadBreadcrumbs()
    {
        _breadcrumbItems =
        [
            MainBreadcrumbFor.Home,
            MainPageProjectsBreadcrumbFor.Index,
            CommonBreadcrumbFor.Active(_pageTitle)
        ];
    }

    private async Task LoadItem()
    {
        try
        {
            _isLoading = true;

            ClearException();

            var query = new GetProjectVerifiesQuery()
            {
                LenderId = _selectedLender,
            };

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

    private async Task LenderValueChanged(Guid lenderId)
    {
        _selectedLender = lenderId;

        await LoadItem();
    }

    private bool IsExpanded(Guid projectId)
    {
        return _expanded.Contains(projectId);
    }

    private int GetRowNumber(ProjectItem item)
    {
        return _items.IndexOf(item) + 1;
    }

    private void Toggle(ProjectItem item)
    {
        if (!_expanded.Add(item.Id))
        {
            _ = _expanded.Remove(item.Id);
        }
    }

    private Task StageVerify(ProjectStageItem item)
    {
        NavigationManager.NavigateTo(MainPageProjectsRouteFor.StageVerify(item.Id));
        return Task.CompletedTask;
    }
}
