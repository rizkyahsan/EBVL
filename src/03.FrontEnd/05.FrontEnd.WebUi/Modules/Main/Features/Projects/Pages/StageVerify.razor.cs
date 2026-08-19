using EBVL.FrontEnd.Logics.Modules.Common.FileStorages.DownloadFileStorage;
using EBVL.FrontEnd.Logics.Modules.Main.Projects.CompleteProjectStage;
using EBVL.FrontEnd.Logics.Modules.Main.Projects.GetProjectVerify;
using EBVL.FrontEnd.Logics.Modules.Main.Projects.ReviewProjectStage;
using EBVL.FrontEnd.Logics.Modules.Main.Projects.RevisionProjectLenderReq;
using EBVL.Shared.Dto.Modules.Main.Projects.CompleteProjectStage;
using EBVL.Shared.Dto.Modules.Main.Projects.GetProjectVerify;
using Microsoft.JSInterop;

namespace EBVL.FrontEnd.WebUi.Modules.Main.Features.Projects.Pages;

public partial class StageVerify
{
    [Inject]
    public required IJSRuntime JSRuntime { get; init; }

    [Parameter]
    public Guid Id { get; init; }

    private ProjectStageItem _item = default!;
    private readonly HashSet<Guid> _expanded = [];
    private readonly HashSet<Guid> _expanded2 = [];
    private bool _isDrawerProjectLenderHistoriesOpen;
    private List<ProjectLenderHistoryItem>? _selectedProjectLenderHistories;
    private List<string>? _selectedLenderEmails;
    private string? _selectedLender;
    private bool _isDrawerRevisionOpen;
    private RevisionProjectLenderReqCommand? _modelRevision;
    private CompleteProjectStageCommand? _modelComplete;
    private bool CanReview => _item is not null
        && _item.StatusCode == StatusesCodeFor.ProjectStageOnProgress
        && _item.DueDate.HasValue && _item.DueDate.Value <= DateTimeOffset.Now;
    private bool CanComplete => _modelComplete is not null
        && _modelComplete.ProjectLenderReqs.All(x => x.StatusCode is
            StatusesCodeFor.ProjectLenderReqAccept or
            StatusesCodeFor.ProjectLenderReqReject);

    protected override async Task OnParametersSetAsync()
    {
        _pageTitle = $"{ProjectStagesDisplayTextFor.Stage} {CommonDisplayTextFor.Verify}";

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

            var query = new GetProjectVerifyQuery() { Id = Id };

            var response = await Sender.Send(query);

            _item = response.Item;
            _modelComplete = new()
            {
                Id = _item.Id,
                ProjectLenderReqs = [.. _item.ProjectLenderReqs
                .Select(x => new CompleteProjectLenderReqRequest
                {
                    Id = x.Id,
                    StatusCode = x.StatusCode
                })]
            };

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

    private void ShowDrawerProjectLenderHistories(ProjectLenderReqItem item)
    {
        _selectedProjectLenderHistories = item.ProjectLenderHistories;
        _selectedLenderEmails = item.ProjectLenderEmails;
        _isDrawerProjectLenderHistoriesOpen = true;
    }

    private void ShowDrawerRevision(ProjectLenderReqItem item)
    {
        _selectedLender = item.ProjectLenderName;

        _modelRevision = new RevisionProjectLenderReqCommand()
        {
            Id = item.Id,
            Remarks = string.Empty
        };

        _isDrawerRevisionOpen = true;
    }

    private void Approve(ProjectLenderReqItem item)
    {
        item.StatusCode = StatusesCodeFor.ProjectLenderReqAccept;
        item.StatusName = "Accept";

        var model = _modelComplete!.ProjectLenderReqs
            .Single(x => x.Id == item.Id);

        model.StatusCode = StatusesCodeFor.ProjectLenderReqAccept;

        StateHasChanged();
    }

    private void Reject(ProjectLenderReqItem item)
    {
        item.StatusCode = StatusesCodeFor.ProjectLenderReqReject;
        item.StatusName = "Reject";

        var model = _modelComplete!.ProjectLenderReqs
            .Single(x => x.Id == item.Id);

        model.StatusCode = StatusesCodeFor.ProjectLenderReqReject;

        StateHasChanged();
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

    private async Task StageReview()
    {
        ClearException();

        var dialogResult = await DialogService.ShowMessageBox(
           "Stage Review",
            $"Do you want to mark this {ProjectStagesDisplayTextFor.ProjectStage} as completion?",
           yesText: CommonDisplayTextFor.Yes,
           noText: CommonDisplayTextFor.No,
           options: new DialogOptions { MaxWidth = MaxWidth.ExtraSmall });

        if (dialogResult is true)
        {
            try
            {
                _isLoading = true;

                await Sender.Send(new ReviewProjectStageCommand
                {
                    Id = Id
                });

                Snackbar.AddSuccess(SuccessMessageFor.Action(ProjectStagesDisplayTextFor.ProjectStage, _item.Name, "Reviewed"));

                await LoadItem();
                await InvokeAsync(StateHasChanged);
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

    private async Task StageComplete()
    {
        ClearException();

        var dialogResult = await DialogService.ShowMessageBox(
           "Stage Completed",
            $"Do you want to mark this {ProjectStagesDisplayTextFor.ProjectStage} as complete?",
           yesText: CommonDisplayTextFor.Yes,
           noText: CommonDisplayTextFor.No,
           options: new DialogOptions { MaxWidth = MaxWidth.ExtraSmall });

        if (dialogResult is true)
        {
            try
            {
                _isLoading = true;

                if (_modelComplete is not null)
                {
                    await Sender.Send(_modelComplete);

                    Snackbar.AddSuccess(SuccessMessageFor.Action(ProjectStagesDisplayTextFor.ProjectStage, _item.Name, "Completed"));

                    NavigationManager.NavigateTo(MainPageProjectsRouteFor.Index);
                }
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
    }

    private Task Cancel()
    {
        NavigationManager.NavigateTo(MainPageProjectsRouteFor.Index);
        return Task.CompletedTask;
    }
}
