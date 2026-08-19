using EBVL.FrontEnd.Logics.Modules.Main.Projects.CreateProject;
using EBVL.Shared.Dto.Common.FileStorages;
using EBVL.Shared.Dto.Modules.Main.Projects.CreateProject;
using EBVL.Shared.Statics.Common;
using Microsoft.JSInterop;

namespace EBVL.FrontEnd.WebUi.Modules.Main.Features.Projects.Pages;

public partial class Create
{
    [Inject]
    public required IJSRuntime JSRuntime { get; init; }

    private bool _isDrawerUploadAttachmentOpen;
    private Guid _selectedLender = Guid.Empty;
    private readonly List<Guid> _currentLender = [];
    private MudForm _form = default!;
    private CreateProjectCommand _model = default!;
    private CreateProjectCommandValidator _validator = default!;
    private CreateProjectAttachmentRequest? _selectedAttachment;

    protected override async Task OnInitializedAsync()
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

    private Task LoadItem()
    {
        try
        {
            _isLoading = true;

            ClearException();

            _model = new()
            {
                Title = string.Empty,
                Desc = string.Empty,
                Objective = string.Empty,
                FinanceType = string.Empty,
                ProjectStage = new CreateProjectStageRequest()
                {
                    StageName = "Preparation Stage",
                    StageDesc = "Gathering Information Lender",
                    DueDate = TimeZoneInfo.ConvertTime(DateTimeOffset.Now.AddMonths(2), TimezoneFor.WibTimeZone),
                },
                ProjectLenders = [],
                ProjectAttachments = [],
                ProjectReqs = []
            };

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

        return Task.CompletedTask;
    }

    private void AddProjectLenders()
    {
        if (_selectedLender == Guid.Empty)
        {
            return;
        }

        _model.ProjectLenders.Add(new CreateProjectLenderRequest
        {
            LenderId = _selectedLender
        });

        _currentLender.Add(_selectedLender);

        // reset selector
        _selectedLender = Guid.Empty;

        StateHasChanged();
    }

    private void DeleteProjectLenders(CreateProjectLenderRequest item)
    {
        _ = _model.ProjectLenders.Remove(item);
        _ = _currentLender.Remove(item.LenderId);

        StateHasChanged();
    }

    private void AddProjectAttachments()
    {
        _model.ProjectAttachments.Add(new CreateProjectAttachmentRequest
        {
            AttachmentName = string.Empty,
            AttachmentDesc = string.Empty,
            AttachmentSortNo = _model.ProjectAttachments.Count + 1,
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

    private void DeleteProjectAttachmentFile(CreateProjectAttachmentRequest item)
    {
        item.File = null;

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
            IsRequired = true
        });

        StateHasChanged();
    }

    private void DeleteProjectReqs(CreateProjectReqRequest item)
    {
        _ = _model.ProjectReqs.Remove(item);

        StateHasChanged();
    }

    private async Task CreateProject()
    {
        ClearException();

        try
        {
            _isLoading = true;

            await _form.RunValidation();

            if (_form.IsValid)
            {
                _ = await Sender.Send(_model);

                Snackbar.AddSuccess(SuccessMessageFor.Created(ProjectsDisplayTextFor.Project, _model.Title));

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
