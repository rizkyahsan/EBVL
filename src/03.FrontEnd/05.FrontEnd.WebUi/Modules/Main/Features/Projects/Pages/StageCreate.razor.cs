using EBVL.FrontEnd.Logics.Modules.Common.FileStorages.DownloadFileStorage;
using EBVL.FrontEnd.Logics.Modules.Main.Projects.CreateProjectStage;
using EBVL.FrontEnd.Logics.Modules.Main.Projects.GetLastProjectStage;
using EBVL.Shared.Dto.Common.FileStorages;
using EBVL.Shared.Dto.Modules.Main.Projects.CreateProjectStage;
using Microsoft.JSInterop;
using ProjectStageItem = EBVL.Shared.Dto.Modules.Main.Projects.GetLastProjectStage.ProjectStageItem;

namespace EBVL.FrontEnd.WebUi.Modules.Main.Features.Projects.Pages;

public partial class StageCreate
{
    [Inject]
    public required IJSRuntime JSRuntime { get; init; }

    [Parameter]
    public Guid Id { get; init; }

    private bool _isDrawerUploadAttachmentOpen;
    private MudForm _form = default!;
    private ProjectStageItem _item = default!;
    private CreateProjectStageCommand _model = default!;
    private CreateProjectStageCommandValidator _validator = default!;
    private CreateProjectAttachmentRequest? _selectedAttachment;

    protected override async Task OnParametersSetAsync()
    {
        _pageTitle = CommonDisplayTextFor.Create;

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

            var query = new GetLastProjectStageQuery() { Id = Id };

            var response = await Sender.Send(query);
            _item = response.Item;

            _model = new()
            {
                ProjectId = Id,
                Name = string.Empty,
                Desc = string.Empty,
                DueDate = null,
                ProjectAttachments = [],
                ProjectReqs = []
            };

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

    private void AddProjectAttachments()
    {
        _model.ProjectAttachments.Add(new CreateProjectAttachmentRequest
        {
            AttachmentName = string.Empty,
            AttachmentDesc = string.Empty,
            AttachmentSortNo = _model.ProjectAttachments.Count + 1,
            FileStorageId = Guid.Empty,
            File = null
        });

        StateHasChanged();
    }

    private void ShowDrawerUploadAttachment(CreateProjectAttachmentRequest item)
    {
        _selectedAttachment = item;
        _isDrawerUploadAttachmentOpen = true;
    }

    private void FileUploaded(FileItem file)
    {
        _ = _selectedAttachment?.File = file;

        StateHasChanged();
    }

    private async Task DownloadFile(CreateProjectAttachmentRequest item)
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

    private void DeleteProjectAttachmentFile(CreateProjectAttachmentRequest item)
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

    private void DeleteProjectAttachments(CreateProjectAttachmentRequest item)
    {
        _ = _model.ProjectAttachments.Remove(item);

        StateHasChanged();
    }

    private void AddProjectReqs()
    {
        _model.ProjectReqs.Add(new CreateProjectReqRequest
        {
            ReqName = string.Empty,
            ReqDesc = string.Empty,
            ReqSortNo = _model.ProjectReqs.Count + 1,
            IsRequired = false
        });

        StateHasChanged();
    }

    private void DeleteProjectReqs(CreateProjectReqRequest item)
    {
        _ = _model.ProjectReqs.Remove(item);

        StateHasChanged();
    }

    private async Task CreateStageProject()
    {
        ClearException();

        try
        {
            _isLoading = true;

            await _form.RunValidation();

            if (_form.IsValid)
            {
                _ = await Sender.Send(_model);

                Snackbar.AddSuccess(SuccessMessageFor.Created(ProjectStagesDisplayTextFor.ProjectStage, _model.Name));

                NavigationManager.NavigateTo(MainPageProjectsRouteFor.Index);
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

    private Task Cancel()
    {
        NavigationManager.NavigateTo(MainPageProjectsRouteFor.Index);
        return Task.CompletedTask;
    }
}
