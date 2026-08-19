using EBVL.FrontEnd.Logics.Modules.Common.FileStorages.DownloadFileStorage;
using EBVL.FrontEnd.Logics.Modules.Main.MyProjects.GetMyProjectStage;
using EBVL.Shared.Dto.Modules.Main.MyProjects.GetMyProjectStage;
using Microsoft.JSInterop;

namespace EBVL.FrontEnd.WebUi.Modules.Main.Features.MyProjects.Pages;

public partial class StageDetails
{
    [Inject]
    public required IJSRuntime JSRuntime { get; init; }

    [Parameter]
    public Guid Id { get; init; }

    private MyProjectStageItem _item = default!;
    private readonly HashSet<Guid> _expanded = [];
    private bool _isDrawerProjectLenderHistoriesOpen;
    private List<MyProjectLenderHistoryItem>? _selectedProjectLenderHistories;
    private int _totalRequiredCount;
    private int _uploadedRequiredCount;
    private MudBlazor.Severity _uploadStatusSeverity;

    protected override async Task OnParametersSetAsync()
    {
        _pageTitle = $"{ProjectStagesDisplayTextFor.Stage} {CommonDisplayTextFor.View}";

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

            var query = new GetMyProjectStageQuery() { Id = Id };

            var response = await Sender.Send(query);

            _item = response.Item;

            _breadcrumbItems.RemoveAt(_breadcrumbItems.Count - 1);
            _breadcrumbItems.Add(CommonBreadcrumbFor.Active(_pageTitle));

            _selectedProjectLenderHistories = [.. _item.ProjectLenderReq.ProjectLenderHistories];
            _totalRequiredCount = _item.ProjectLenderReq.ProjectReqItems.Count(x => x.IsRequired);
            _uploadedRequiredCount = _item.ProjectLenderReq.ProjectReqItems
                .Count(x => x.IsRequired && x.ProjectLenderReqFiles.Count != 0);
            _uploadStatusSeverity = _uploadedRequiredCount == _totalRequiredCount ? MudBlazor.Severity.Success : MudBlazor.Severity.Info;
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

    private IEnumerable<MyProjectLenderReqFileItem> GetFiles(Guid projectReqId)
    {
        return _item.ProjectLenderReq.ProjectReqItems.Where(x => x.Id == projectReqId).SelectMany(x => x.ProjectLenderReqFiles);
    }

    private void Toggle(MyProjectReqItem item)
    {
        if (!_expanded.Add(item.Id))
        {
            _ = _expanded.Remove(item.Id);
        }
    }

    private void ShowDrawerProjectLenderHistories()
    {
        _isDrawerProjectLenderHistoriesOpen = true;
    }

    private async Task DownloadFileAttachment(MyProjectAttachmentItem item)
    {
        if (item.FileStorageId != Guid.Empty)
        {
            var response = await Sender.Send(new DownloadFileStorageQuery()
            {
                FileStorageId = item.FileStorageId
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

    private async Task DownloadFileReq(MyProjectLenderReqFileItem item)
    {
        if (item.FileStorageId != Guid.Empty)
        {
            var response = await Sender.Send(new DownloadFileStorageQuery()
            {
                FileStorageId = item.FileStorageId
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

    private Task Cancel()
    {
        NavigationManager.NavigateTo(MainPageMyProjectsRouteFor.Index);
        return Task.CompletedTask;
    }
}
