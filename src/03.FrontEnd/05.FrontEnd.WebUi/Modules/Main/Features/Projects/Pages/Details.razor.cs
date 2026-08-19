using EBVL.FrontEnd.Logics.Modules.Common.FileStorages.DownloadFileStorage;
using EBVL.FrontEnd.Logics.Modules.Main.Projects.GetProject;
using EBVL.Shared.Dto.Modules.Main.Projects.GetProject;
using Microsoft.JSInterop;

namespace EBVL.FrontEnd.WebUi.Modules.Main.Features.Projects.Pages;

public partial class Details
{
    [Inject]
    public required IJSRuntime JSRuntime { get; init; }

    [Parameter]
    public Guid Id { get; init; }

    private ProjectItem _item = default!;
    public Dictionary<Guid, ProjectStageItem> _projectStageList = [];
    public required string _width;
    public int _colspanProgressBar;

    protected override async Task OnParametersSetAsync()
    {
        _pageTitle = CommonDisplayTextFor.View;

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

    private void BuildPageState()
    {
        _projectStageList = _item.ProjectStages
            .OrderBy(x => x.Level)
            .ToDictionary(x => x.Id, y => y);

        _colspanProgressBar = _item.ProjectStages.Count + 2;

        var stepPercentage = 100d / _colspanProgressBar;
        _width = $"{stepPercentage:#.##}%";
    }

    private async Task LoadItem()
    {
        try
        {
            _isLoading = true;

            ClearException();

            var query = new GetProjectQuery() { Id = Id };

            var response = await Sender.Send(query);

            _item = response.Item;

            _breadcrumbItems.RemoveAt(_breadcrumbItems.Count - 1);
            _breadcrumbItems.Add(CommonBreadcrumbFor.Active(_pageTitle));

            BuildPageState();
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

    private async Task DownloadFileLender(ProjectLenderItem item)
    {
        if (!item.FileStorageId.HasValue)
        {
            return;
        }

        var response = await Sender.Send(new DownloadFileStorageQuery()
        {
            FileStorageId = item.FileStorageId.Value
        });

        if (response is not null)
        {

            await JSRuntime.InvokeVoidAsync(
                "downloadFile",
                response.FileName,
                response.FileContentType,
                Convert.ToBase64String(response.FileContent));
        }
    }

    private Task StageDetails(ProjectStageItem item)
    {
        NavigationManager.NavigateTo(MainPageProjectsRouteFor.StageDetails(item.Id));
        return Task.CompletedTask;
    }

    private Task Cancel()
    {
        NavigationManager.NavigateTo(MainPageProjectsRouteFor.Index);
        return Task.CompletedTask;
    }
}
