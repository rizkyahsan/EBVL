using EBVL.FrontEnd.Logics.Modules.Common.FileStorages.DownloadFileStorage;
using EBVL.FrontEnd.Logics.Modules.Main.Projects.GetProjectStage;
using EBVL.Shared.Dto.Modules.Main.Projects.GetProjectStage;
using Microsoft.JSInterop;

namespace EBVL.FrontEnd.WebUi.Modules.Main.Features.Projects.Pages;

public partial class StageDetails
{
    [Inject]
    public required IJSRuntime JSRuntime { get; init; }

    [Parameter]
    public Guid Id { get; init; }

    private ProjectStageItem _item = default!;
    private readonly HashSet<Guid> _expanded = [];
    private readonly HashSet<Guid> _expanded2 = [];
    private bool _isDrawerProjectStageLenderHistoriesOpen;
    private List<ProjectLenderHistoryItem>? _selectedProjectLenderHistories;
    private List<string>? _selectedLenderEmails;

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

            var query = new GetProjectStageQuery() { Id = Id };

            var response = await Sender.Send(query);

            _item = response.Item;

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

    private IEnumerable<ProjectLenderReqFileItem> GetFiles(Guid projectLenderId, Guid projectReqId)
    {
        return _item.ProjectLenderReqs.Where(x => x.ProjectLenderId == projectLenderId)
            .SelectMany(x => x.ProjectReqItems).Where(x => x.Id == projectReqId)
            .SelectMany(x => x.ProjectLenderReqFiles);
    }

    private bool IsExpanded(Guid projectLenderReqItemId)
    {
        return _expanded.Contains(projectLenderReqItemId);
    }

    private int GetRowNumber(ProjectLenderReqItem item)
    {
        return _item.ProjectLenderReqs.ToList().IndexOf(item) + 1;
    }

    private void Toggle(ProjectLenderReqItem item)
    {
        if (!_expanded.Add(item.Id))
        {
            _ = _expanded.Remove(item.Id);
        }
    }

    private bool IsExpanded2(Guid projectReqItemId)
    {
        return _expanded2.Contains(projectReqItemId);
    }

    private void Toggle2(ProjectReqItem item)
    {
        if (!_expanded2.Add(item.Id))
        {
            _ = _expanded2.Remove(item.Id);
        }
    }

    private async Task DownloadFileAttachment(ProjectAttachmentItem item)
    {
        if (item.FileStorageId == Guid.Empty)
        {
            return;
        }

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

    private void ShowDrawerProjectLenderHistories(ProjectLenderReqItem item)
    {
        _selectedProjectLenderHistories = item.ProjectLenderHistories;
        _selectedLenderEmails = item.ProjectLenderEmails;
        _isDrawerProjectStageLenderHistoriesOpen = true;
    }

    private async Task DownloadFileReq(ProjectLenderReqFileItem item)
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
        NavigationManager.NavigateTo(MainPageProjectsRouteFor.Index);
        return Task.CompletedTask;
    }
}
