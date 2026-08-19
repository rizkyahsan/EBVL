using EBVL.FrontEnd.Logics.Modules.Common.FileStorages.DownloadFileStorage;
using EBVL.FrontEnd.Logics.Modules.Main.MyProjects.GetMyProject;
using EBVL.Shared.Dto.Modules.Main.MyProjects.GetMyProject;
using Microsoft.JSInterop;

namespace EBVL.FrontEnd.WebUi.Modules.Main.Features.MyProjects.Pages;

public partial class Details
{
    [Inject]
    public required IJSRuntime JSRuntime { get; init; }

    [Parameter]
    public Guid Id { get; init; }

    private MyProjectItem _item = default!;
    public Dictionary<Guid, MyProjectStageItem> _projectStageList = [];
    public required string _width;
    public int _colspanProgressBar;

    protected override async Task OnParametersSetAsync()
    {
        _pageTitle = CommonDisplayTextFor.Details;

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

            var query = new GetMyProjectQuery() { Id = Id };

            var response = await Sender.Send(query);

            _item = response.Item;

            _breadcrumbItems.RemoveAt(_breadcrumbItems.Count - 1);
            _breadcrumbItems.Add(CommonBreadcrumbFor.Active(_pageTitle));

            _projectStageList = _item.ProjectStages
                .OrderBy(x => x.Level)
                .ToDictionary(x => x.Id, y => y);

            _colspanProgressBar = _item.ProjectStages.Count + 1;

            var stepPercentage = 100d / _colspanProgressBar;
            _width = $"{stepPercentage:#.##}%";
        }
        catch (Exception exception)
        {
            _exception = exception;
        }
        finally
        {
            _isLoading = false;
        }
    }

    private async Task DownloadFileAttachment()
    {
        if (_item.FileStorageId.HasValue)
        {
            var response = await Sender.Send(new DownloadFileStorageQuery()
            {
                FileStorageId = _item.FileStorageId.Value
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
    }

    private Task StageUpdate(MyProjectStageItem item)
    {
        NavigationManager.NavigateTo(MainPageMyProjectsRouteFor.StageUpdate(item.Id));
        return Task.CompletedTask;
    }

    private Task StageDetails(MyProjectStageItem item)
    {
        NavigationManager.NavigateTo(MainPageMyProjectsRouteFor.StageDetails(item.Id));
        return Task.CompletedTask;
    }

    private Task Cancel()
    {
        NavigationManager.NavigateTo(MainPageMyProjectsRouteFor.Index);
        return Task.CompletedTask;
    }
}
