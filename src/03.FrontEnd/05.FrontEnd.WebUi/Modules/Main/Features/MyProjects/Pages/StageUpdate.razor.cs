using EBVL.FrontEnd.Logics.Modules.Common.FileStorages.DownloadFileStorage;
using EBVL.FrontEnd.Logics.Modules.Main.MyProjects.GetMyProjectStage;
using EBVL.FrontEnd.Logics.Modules.Main.MyProjects.UpdateMyProjectStage;
using EBVL.Shared.Dto.Common.FileStorages;
using EBVL.Shared.Dto.Modules.Main.MyProjects.GetMyProjectStage;
using EBVL.Shared.Dto.Modules.Main.MyProjects.UpdateMyProjectStage;
using Microsoft.JSInterop;

namespace EBVL.FrontEnd.WebUi.Modules.Main.Features.MyProjects.Pages;

public partial class StageUpdate
{
    [Inject]
    public required IJSRuntime JSRuntime { get; init; }

    [Parameter]
    public Guid Id { get; init; }

    private MudForm _form = default!;
    private MyProjectStageItem _item = default!;
    private UpdateMyProjectStageCommand _model = default!;
    private UpdateMyProjectStageCommandValidator _validator = default!;
    private MyProjectReqItem? _selectedReq;
    private readonly HashSet<Guid> _expanded = [];
    private Dictionary<Guid, string>? _fileNameProjectReq;
    private bool _isDrawerUploadReqOpen;
    private bool _isDrawerProjectLenderHistoriesOpen;
    private List<MyProjectLenderHistoryItem>? _selectedProjectLenderHistories;
    private int _totalRequiredCount;
    private int _uploadedRequiredCount;
    private MudBlazor.Severity _uploadStatusSeverity;
    private bool _allowSubmit = false;

    protected override async Task OnParametersSetAsync()
    {
        _pageTitle = $"{ProjectStagesDisplayTextFor.Stage} {CommonDisplayTextFor.Update}";

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

            if (!_item.IsPicLender || !_item.IsAllowUpdate)
            {
                NavigationManager.NavigateTo(MainPageMyProjectsRouteFor.StageDetails(_item.Id));
            }

            _model = new()
            {
                Id = _item.ProjectLenderReq.Id,
                ProjectId = _item.ProjectId,
                ProjectLenderId = _item.ProjectLenderReq.ProjectLenderId,
                ProjectStageId = _item.Id,
                IsSubmitted = false,
                Remarks = string.Empty,
                ProjectLenderReqFiles = [.. _item.ProjectLenderReq.ProjectReqItems
                .SelectMany(req =>
                {
                    return req.ProjectLenderReqFiles.Select(file => new UpdateMyProjectLenderReqFileRequest
                    {
                        Id = file.Id,
                        ProjectReqId = req.Id,
                        FileStorageId = file.FileStorageId,
                        File = null
                    });
                })]
            };

            #region Set Value
            _selectedProjectLenderHistories = [.. _item.ProjectLenderReq.ProjectLenderHistories];
            _fileNameProjectReq = _item.ProjectLenderReq.ProjectReqItems
                .SelectMany(x => x.ProjectLenderReqFiles).GroupBy(x => x.FileStorageId)
                .ToDictionary(x => x.Key, x => x.First().FileStorageName);
            _totalRequiredCount = _item.ProjectLenderReq.ProjectReqItems.Count(x => x.IsRequired);
            _uploadedRequiredCount = _item.ProjectLenderReq.ProjectReqItems
                .Count(x => x.IsRequired && GetFiles(x.Id).Any());
            _uploadStatusSeverity = _uploadedRequiredCount == _totalRequiredCount ? MudBlazor.Severity.Success : MudBlazor.Severity.Info;
            _allowSubmit = _item.ProjectLenderReq.ProjectReqItems
                .Where(x => x.IsRequired).All(x => GetFiles(x.Id).Any());
            #endregion

            _breadcrumbItems.RemoveAt(_breadcrumbItems.Count - 1);
            _breadcrumbItems.Add(CommonBreadcrumbFor.Active(_pageTitle));

            _validator = new();
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

    private string GetProjectReqFileName(Guid id)
    {
        return _fileNameProjectReq?.GetValueOrDefault(id) ?? string.Empty;
    }

    private IEnumerable<UpdateMyProjectLenderReqFileRequest> GetFiles(Guid projectReqId)
    {
        return _model.ProjectLenderReqFiles.Where(x => x.ProjectReqId == projectReqId);
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

    private void ShowDrawerUploadReq(MyProjectReqItem item)
    {
        _selectedReq = item;
        _isDrawerUploadReqOpen = true;
    }

    private async Task FileUploaded(FileItem file)
    {
        if (_selectedReq is null)
        {
            return;
        }

        _model.ProjectLenderReqFiles.Add(
            new UpdateMyProjectLenderReqFileRequest
            {
                Id = Guid.Empty,
                ProjectReqId = _selectedReq.Id,
                FileStorageId = Guid.Empty,
                File = file
            });

        await InvokeAsync(StateHasChanged);
    }

    private async Task DownloadFileReq(UpdateMyProjectLenderReqFileRequest item)
    {
        if (item.FileStorageId != Guid.Empty && item.File is null)
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
        else
        {
            var file = item.File;

            if (file is not null)
            {
                await JSRuntime.InvokeVoidAsync(
                    "downloadFile",
                    file.FileName,
                    file.ContentType,
                    Convert.ToBase64String(file.FileContent));
            }
        }
    }

    private async Task DeleteProjectLenderReqFile(UpdateMyProjectLenderReqFileRequest item)
    {
        _ = _model.ProjectLenderReqFiles.Remove(item);

        await InvokeAsync(StateHasChanged);
    }

    private async Task UpdateStageProject()
    {
        ClearException();

        try
        {
            _isLoading = true;

            _model.IsSubmitted = false;
            await _form.RunValidation();

            if (_form.IsValid)
            {
                await Sender.Send(_model);

                if (_item.Level > 0)
                {
                    Snackbar.AddSuccess(SuccessMessageFor.Submitted($"Stage {ProjectsDisplayTextFor.Project}", _item.Name));
                }
                else
                {
                    Snackbar.AddSuccess(SuccessMessageFor.Updated($"Stage {ProjectsDisplayTextFor.Project}", _item.Name));
                }

                await LoadItem();

                await InvokeAsync(StateHasChanged);
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

    private async Task SubmitStageProject()
    {
        ClearException();

        try
        {
            _isLoading = true;

            _model.IsSubmitted = true;
            await _form.RunValidation();

            if (_form.IsValid)
            {
                await Sender.Send(_model);

                Snackbar.AddSuccess(SuccessMessageFor.Submitted($"Stage {ProjectsDisplayTextFor.Project}", _item.Name));
                NavigationManager.NavigateTo(MainPageMyProjectsRouteFor.StageDetails(Id));
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

    private Task Cancel()
    {
        NavigationManager.NavigateTo(MainPageMyProjectsRouteFor.Index);
        return Task.CompletedTask;
    }
}
