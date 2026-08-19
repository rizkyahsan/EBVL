using EBVL.FrontEnd.Logics.Modules.Main.Projects.GetProjects;
using EBVL.FrontEnd.Logics.Modules.Main.Projects.PublishProjectStage;
using EBVL.Shared.Dto.Modules.Main.Projects.GetProjects;
using Microsoft.JSInterop;

namespace EBVL.FrontEnd.WebUi.Modules.Main.Features.Projects.Pages;

public partial class Index
{
    [Inject]
    public required IJSRuntime JSRuntime { get; init; }

    private List<ProjectItem> _items = [];
    private readonly HashSet<Guid> _expanded = [];
    private Guid _selectedLender = Guid.Empty;
    private string _selectedStatus = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        _pageTitle = $"Manage";

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

            var query = new GetProjectsQuery()
            {
                LenderId = _selectedLender,
                StatusCode = _selectedStatus
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

    private async Task StatusValueChanged(string statusCode)
    {
        _selectedStatus = statusCode;

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
        if (_expanded.Contains(item.Id))
        {
            _ = _expanded.Remove(item.Id);
        }
        else
        {
            _ = _expanded.Add(item.Id);
        }
    }

    private static bool IsCompleteProject(ProjectItem item)
    {
        return item.StatusCode is StatusesCodeFor.ProjectComplete or StatusesCodeFor.ProjectCancel;
    }

    private static bool CanEditStage(ProjectStageItem item)
    {
        return item.StatusCode == StatusesCodeFor.ProjectStageDraft;
    }

    private void Create()
    {
        NavigationManager.NavigateTo(MainPageProjectsRouteFor.Create());
    }

    private void Details(ProjectItem item)
    {
        NavigationManager.NavigateTo(MainPageProjectsRouteFor.Details(item.Id));
    }

    private void Update(ProjectItem item)
    {
        NavigationManager.NavigateTo(MainPageProjectsRouteFor.Update(item.Id));
    }

    private void StageUpdate(ProjectStageItem item)
    {
        NavigationManager.NavigateTo(MainPageProjectsRouteFor.StageUpdate(item.Id));
    }

    private void StageDetails(ProjectStageItem item)
    {
        NavigationManager.NavigateTo(MainPageProjectsRouteFor.StageDetails(item.Id));
    }

    private async Task StagePublish(ProjectStageItem item)
    {
        ClearException();

        var dialogResult = await DialogService.ShowMessageBox(
            "Stage Publish",
            $"Do you want to mark this {item.Name} as Publish?",
            yesText: CommonDisplayTextFor.Yes,
            noText: CommonDisplayTextFor.No,
            options: new DialogOptions { MaxWidth = MaxWidth.ExtraSmall });

        if (dialogResult is true)
        {
            try
            {
                _isLoading = true;

                await Sender.Send(new PublishProjectStageCommand
                {
                    Id = item.Id
                });

                Snackbar.AddSuccess(SuccessMessageFor.Action(ProjectStagesDisplayTextFor.ProjectStage, item.Name, CommonDisplayTextFor.Published));

                await LoadItem();
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
    }
}
