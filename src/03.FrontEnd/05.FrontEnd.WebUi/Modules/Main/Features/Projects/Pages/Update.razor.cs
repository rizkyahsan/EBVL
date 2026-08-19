using EBVL.FrontEnd.Logics.Modules.Common.FileStorages.DownloadFileStorage;
using EBVL.FrontEnd.Logics.Modules.Main.Projects.CompleteProject;
using EBVL.FrontEnd.Logics.Modules.Main.Projects.GetProject;
using EBVL.FrontEnd.Logics.Modules.Main.Projects.PublishProjectStage;
using EBVL.FrontEnd.Logics.Modules.Main.Projects.UpdateProject;
using EBVL.Shared.Dto.Common.FileStorages;
using EBVL.Shared.Dto.Modules.Main.Projects.CompleteProject;
using EBVL.Shared.Dto.Modules.Main.Projects.GetProject;
using EBVL.Shared.Dto.Modules.Main.Projects.UpdateProject;
using Microsoft.JSInterop;

namespace EBVL.FrontEnd.WebUi.Modules.Main.Features.Projects.Pages;

public partial class Update
{
    [Inject]
    public required IJSRuntime JSRuntime { get; init; }

    [Parameter]
    public Guid Id { get; init; }

    private ProjectItem _item = default!;
    private UpdateProjectCommand _model = default!;
    private CompleteProjectCommand _modelComplete = default!;
    private Guid _selectedLender2 = Guid.Empty;
    private List<Guid> _currentLender = [];
    private UpdateProjectLenderRequest? _selectedLender;
    private Dictionary<Guid, string>? _nameLender;
    private Dictionary<Guid, string>? _fileNameProjectLender;
    private bool _isDrawerLenderWinnerOpen;
    private bool _isDrawerLenderLoserOpen;
    public Dictionary<Guid, ProjectStageItem> _projectStageList = [];
    public required string _width;
    public int _colspanProgressBar;
    public int _projectStageOnProgressCount = 0;

    private bool CanComplete => _model is not null
        && _model.ProjectLenders.All(x => x.StatusCode is
            StatusesCodeFor.ProjectLenderWin or
            StatusesCodeFor.ProjectLenderLose);

    protected override async Task OnParametersSetAsync()
    {
        _pageTitle = CommonDisplayTextFor.Update;

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
        _nameLender = _item.ProjectLenders
            .ToDictionary(x => x.LenderId, y => y.LenderName);
        _currentLender = [.. _item.ProjectLenders.Select(x => x.LenderId)];
        _fileNameProjectLender = _item.ProjectLenders.Where(x => x.FileStorageId != null)
            .ToDictionary(x => (Guid)x.FileStorageId!, y => y.FileStorageName);

        _projectStageList = _item.ProjectStages
            .OrderBy(x => x.Level)
            .ToDictionary(x => x.Id, y => y);

        _colspanProgressBar = _item.ProjectStages.Count + 2;

        var stepPercentage = 100d / _colspanProgressBar;
        _width = $"{stepPercentage:#.##}%";

        _projectStageOnProgressCount = _item.ProjectStages.Count(x => x.StatusCode != StatusesCodeFor.ProjectStageComplete);
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

            _model = new()
            {
                Id = _item.Id,
                Title = _item.Title,
                Desc = _item.Desc,
                Objective = _item.Objective,
                FinanceType = _item.FinanceType,
                StatusId = _item.StatusId,
                ProjectLenders = [.. _item.ProjectLenders
                .Select(x => new UpdateProjectLenderRequest
                {
                    Id = x.Id,
                    LenderId = x.LenderId,
                    StatusName = x.StatusName,
                    StatusCode = x.StatusCode,
                    Note = x.Note,
                    FileStorageId = x.FileStorageId,
                    File = null
                })]
            };

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

    private string GetLenderName(Guid id)
    {
        return _nameLender?.GetValueOrDefault(id) ?? string.Empty;
    }

    private bool CanShowWinner(UpdateProjectLenderRequest lender)
    {
        if (_item.StatusCode != StatusesCodeFor.ProjectOnProgress)
        {
            return false;
        }

        if (_projectStageOnProgressCount != 0)
        {
            return false;
        }

        return lender.StatusCode is
            StatusesCodeFor.ProjectLenderOnProgress or
            StatusesCodeFor.ProjectLenderLose;
    }

    private bool CanShowFailed(UpdateProjectLenderRequest lender)
    {
        if (_item.StatusCode != StatusesCodeFor.ProjectOnProgress)
        {
            return false;
        }

        if (_projectStageOnProgressCount == 0)
        {
            return lender.StatusCode is
                StatusesCodeFor.ProjectLenderOnProgress or
                StatusesCodeFor.ProjectLenderWin;
        }

        return lender.StatusCode == StatusesCodeFor.ProjectLenderOnProgress;
    }

    private string GetProjectAttachmentFileName(Guid id)
    {
        return _fileNameProjectLender?.GetValueOrDefault(id) ?? string.Empty;
    }

    private void AddProjectLenders()
    {
        if (_selectedLender2 == Guid.Empty)
        {
            return;
        }

        _model.ProjectLenders.Add(new UpdateProjectLenderRequest
        {
            Id = Guid.Empty,
            LenderId = _selectedLender2,
            StatusCode = StatusesCodeFor.ProjectLenderDraft
        });

        _currentLender.Add(_selectedLender2);

        // reset selector
        _selectedLender2 = Guid.Empty;

        StateHasChanged();
    }

    private void ShowDrawerLenderWinnerOpen(UpdateProjectLenderRequest item)
    {
        _selectedLender = item;
        _isDrawerLenderWinnerOpen = true;
    }

    private void ShowDrawerLenderLoserOpen(UpdateProjectLenderRequest item)
    {
        _selectedLender = item;
        _isDrawerLenderLoserOpen = true;
    }

    private void FileWinnerUploaded((string Note, FileItem File) data)
    {
        _ = _selectedLender?.StatusCode = StatusesCodeFor.ProjectLenderWin;
        _ = _selectedLender?.StatusName = "Win";
        _ = _selectedLender?.Note = data.Note;
        _ = _selectedLender?.File = data.File;

        StateHasChanged();
    }

    private void FileLoserUploaded((string Note, FileItem File) data)
    {
        _ = _selectedLender?.StatusCode = StatusesCodeFor.ProjectLenderLose;
        _ = _selectedLender?.StatusName = "Lose";
        _ = _selectedLender?.Note = data.Note;
        _ = _selectedLender?.File = data.File;

        StateHasChanged();
    }

    private async Task DownloadFileLender(UpdateProjectLenderRequest item)
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

    private void DeleteProjectLenderFile(UpdateProjectLenderRequest item)
    {
        if (item.File is not null)
        {
            item.File = null;

            StateHasChanged();
            return;
        }

        if (!item.FileStorageId.HasValue)
        {
            return;
        }

        item.FileStorageId = Guid.Empty;

        StateHasChanged();
    }

    private void DeleteProjectLenders(UpdateProjectLenderRequest item)
    {
        _ = _model.ProjectLenders.Remove(item);
        _ = _currentLender.Remove(item.LenderId);

        StateHasChanged();
    }

    private Task StageCreate(ProjectItem item)
    {
        NavigationManager.NavigateTo(MainPageProjectsRouteFor.StageCreate(item.Id));
        return Task.CompletedTask;
    }

    private Task StageUpdate(ProjectStageItem item)
    {
        NavigationManager.NavigateTo(MainPageProjectsRouteFor.StageUpdate(item.Id));
        return Task.CompletedTask;
    }

    private Task StageDetails(ProjectStageItem item)
    {
        NavigationManager.NavigateTo(MainPageProjectsRouteFor.StageDetails(item.Id));
        return Task.CompletedTask;
    }

    private async Task UpdateProject()
    {
        ClearException();

        try
        {
            _isLoading = true;

            await Sender.Send(_model);

            Snackbar.AddSuccess(SuccessMessageFor.Updated(ProjectsDisplayTextFor.Project, _item.Title));

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

    private async Task CompleteProject()
    {
        ClearException();
        var dialogResult = await DialogService.ShowMessageBox(
          "Project Completed",
           $"Do you want to mark this {_model.Title} as complete?",
          yesText: CommonDisplayTextFor.Yes,
          noText: CommonDisplayTextFor.No,
          options: new DialogOptions { MaxWidth = MaxWidth.ExtraSmall });

        if (dialogResult is true)
        {
            try
            {
                _isLoading = true;

                _modelComplete = new()
                {
                    Id = _model.Id,
                    ProjectLenders = [.. _model.ProjectLenders
                    .Select(x => new CompleteProjectLenderRequest
                    {
                        Id = x.Id,
                        StatusCode = x.StatusCode,
                        Note = x.Note,
                        FileStorageId = x.FileStorageId ?? Guid.Empty,
                        File = x.File
                    })]
                };

                await Sender.Send(_modelComplete);

                Snackbar.AddSuccess(SuccessMessageFor.Action(ProjectsDisplayTextFor.Project, _item.Title, "Completed"));

                NavigationManager.NavigateTo(MainPageProjectsRouteFor.Details(_item.Id));
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

    private async Task StagePublish(ProjectStageItem item)
    {
        ClearException();

        var dialogResult = await DialogService.ShowMessageBox("Stage Publish",
            $"Do you want to mark this {_model.Title} as Publish?",
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

    private Task Cancel()
    {
        NavigationManager.NavigateTo(MainPageProjectsRouteFor.Index);
        return Task.CompletedTask;
    }
}
