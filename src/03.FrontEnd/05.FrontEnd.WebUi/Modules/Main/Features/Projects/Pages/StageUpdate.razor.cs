using EBVL.FrontEnd.Logics.Modules.Common.FileStorages.DownloadFileStorage;
using EBVL.FrontEnd.Logics.Modules.Main.Projects.GetProjectStage;
using EBVL.FrontEnd.Logics.Modules.Main.Projects.PublishProjectStage;
using EBVL.FrontEnd.Logics.Modules.Main.Projects.UpdateProjectStage;
using EBVL.Shared.Dto.Common.FileStorages;
using EBVL.Shared.Dto.Modules.Main.Projects.GetProjectStage;
using EBVL.Shared.Dto.Modules.Main.Projects.UpdateProjectStage;
using Microsoft.JSInterop;

namespace EBVL.FrontEnd.WebUi.Modules.Main.Features.Projects.Pages;

public partial class StageUpdate
{
    [Inject]
    public required IJSRuntime JSRuntime { get; init; }

    [Parameter]
    public Guid Id { get; init; }

    private bool _isDrawerUploadAttachmentOpen;
    private MudForm _form = default!;
    private ProjectStageItem _item = default!;
    private UpdateProjectStageCommand _model = default!;
    private UpdateProjectStageCommandValidator _validator = default!;
    private UpdateProjectAttachmentRequest? _selectedAttachment;
    private Dictionary<Guid, string>? _fileNameProjectAttachment;

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
            MainBreadcrumbFor.Home,
            MainPageProjectsBreadcrumbFor.Index,
            CommonBreadcrumbFor.Active(_pageTitle)
        ];
    }

    private void BuildPageState()
    {
        _fileNameProjectAttachment = _item.ProjectAttachments.ToDictionary(x => x.FileStorageId, y => y.FileStorageName);
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

            _model = new()
            {
                Id = _item.Id,
                Name = _item.Name,
                Desc = _item.Desc,
                DueDate = _item.DueDate,
                ProjectAttachments = [.. _item.ProjectAttachments
                .Select(x => new UpdateProjectAttachmentRequest
                {
                    Id = x.Id,
                    AttachmentName = x.Name,
                    AttachmentDesc = x.Desc,
                    AttachmentSortNo = x.SortNo,
                    FileStorageId = x.FileStorageId,
                    File = null,
                })],
                ProjectReqs = [.. _item.ProjectReqs
                .Select(x => new UpdateProjectReqRequest
                {
                    Id = x.Id,
                    ReqName = x.Name,
                    ReqDesc = x.Desc,
                    ReqSortNo = x.SortNo,
                    IsRequired = x.IsRequired,
                })],
            };

            _breadcrumbItems.RemoveAt(_breadcrumbItems.Count - 1);
            _breadcrumbItems.Add(CommonBreadcrumbFor.Active(_pageTitle));

            BuildPageState();

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

    private string GetProjectAttachmentFileName(Guid id)
    {
        return _fileNameProjectAttachment?.GetValueOrDefault(id) ?? string.Empty;
    }

    private void AddProjectAttachments()
    {
        _model.ProjectAttachments.Add(new UpdateProjectAttachmentRequest
        {
            Id = Guid.Empty,
            AttachmentName = string.Empty,
            AttachmentDesc = string.Empty,
            AttachmentSortNo = _item.ProjectAttachments.Count + 1,
            FileStorageId = Guid.Empty,
            File = null
        });

        StateHasChanged();
    }

    private void ShowDrawerUploadAttachment(UpdateProjectAttachmentRequest item)
    {
        _selectedAttachment = item;
        _isDrawerUploadAttachmentOpen = true;
    }

    private void FileUploaded(FileItem file)
    {
        _ = _selectedAttachment?.File = file;

        StateHasChanged();
    }

    private async Task DownloadFileAttachment(UpdateProjectAttachmentRequest item)
    {
        if (item.File is not null)
        {
            var file = item.File;

            await JSRuntime.InvokeVoidAsync(
                "downloadFile",
                file.FileName,
                file.ContentType,
                Convert.ToBase64String(file.FileContent));
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

    private void DeleteProjectAttachmentFile(UpdateProjectAttachmentRequest item)
    {
        if (item.File is not null)
        {
            item.File = null;

            StateHasChanged();
            return;
        }

        item.FileStorageId = Guid.Empty;

        StateHasChanged();
    }

    private void DeleteProjectAttachments(UpdateProjectAttachmentRequest item)
    {
        _ = _model.ProjectAttachments.Remove(item);

        StateHasChanged();
    }

    private void AddProjectReqs()
    {
        _model.ProjectReqs.Add(new UpdateProjectReqRequest
        {
            Id = Guid.Empty,
            ReqName = string.Empty,
            ReqDesc = string.Empty,
            ReqSortNo = _item.ProjectReqs.Count + 1,
            IsRequired = false
        });

        StateHasChanged();
    }

    private void DeleteProjectReqs(UpdateProjectReqRequest item)
    {
        _ = _model.ProjectReqs.Remove(item);

        StateHasChanged();
    }

    private async Task UpdateStageProject()
    {
        ClearException();

        try
        {
            _isLoading = true;

            await _form.RunValidation();

            if (_form.IsValid)
            {
                await Sender.Send(_model);

                Snackbar.AddSuccess(SuccessMessageFor.Updated(ProjectStagesDisplayTextFor.ProjectStage, _item.Name));

                NavigationManager.NavigateTo(MainPageProjectsRouteFor.StageUpdate(Id));
            }
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

    private async Task StagePublish()
    {
        ClearException();

        var dialogResult = await DialogService.ShowMessageBox("Stage Publish",
            $"Do you want to mark this {_model.Name} as Publish?",
            yesText: CommonDisplayTextFor.Yes,
            noText: CommonDisplayTextFor.No,
            options: new DialogOptions { MaxWidth = MaxWidth.ExtraSmall });

        if (dialogResult is true)
        {
            try
            {
                _isLoading = true;

                await _form.RunValidation();

                if (_form.IsValid)
                {
                    await Sender.Send(_model);

                    await Sender.Send(new PublishProjectStageCommand
                    {
                        Id = Id
                    });

                    Snackbar.AddSuccess(SuccessMessageFor.Action(ProjectStagesDisplayTextFor.ProjectStage, _item.Name, CommonDisplayTextFor.Published));

                    NavigationManager.NavigateTo(MainPageProjectsRouteFor.StageDetails(Id));
                }
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

    private Task Cancel()
    {
        NavigationManager.NavigateTo(MainPageProjectsRouteFor.Index);
        return Task.CompletedTask;
    }
}
