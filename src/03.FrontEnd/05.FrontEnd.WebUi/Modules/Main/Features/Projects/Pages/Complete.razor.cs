using EBVL.FrontEnd.Logics.Modules.Common.FileStorages.DownloadFileStorage;
using EBVL.FrontEnd.Logics.Modules.Main.Projects.DeleteProjectFile;
using EBVL.FrontEnd.Logics.Modules.Main.Projects.GetProjectCompletes;
using EBVL.FrontEnd.Logics.Modules.Main.Projects.UploadProjectFile;
using EBVL.Shared.Dto.Common.FileStorages;
using EBVL.Shared.Dto.Modules.Main.Projects.GetProjectCompletes;
using Microsoft.JSInterop;

namespace EBVL.FrontEnd.WebUi.Modules.Main.Features.Projects.Pages;

public partial class Complete
{
    [Inject]
    public required IJSRuntime JSRuntime { get; init; }

    private List<ProjectItem> _items = [];
    private readonly HashSet<Guid> _expanded = [];
    private Guid _selectedLender = Guid.Empty;
    private ProjectItem? _selectedProject;
    private bool _isDrawerUploadOpen;

    protected override async Task OnInitializedAsync()
    {
        _pageTitle = $"Archive";

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

            var query = new GetProjectCompletesQuery()
            {
                LenderId = _selectedLender,
            };

            var response = await Sender.Send(query);

            _items = [.. response.Items];

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

    private async Task LenderValueChanged(Guid lenderId)
    {
        _selectedLender = lenderId;

        await LoadItem();
    }

    private bool IsExpanded(Guid projectId)
    {
        return _expanded.Contains(projectId);
    }

    private int GetRowNumber(ProjectItem item)
    {
        return _items.IndexOf(item) + 1;
    }

    private void Toggle(ProjectItem item)
    {
        if (!_expanded.Add(item.Id))
        {
            _ = _expanded.Remove(item.Id);
        }
    }

    private void ShowDrawerUpload(ProjectItem item)
    {
        _selectedProject = item;
        _isDrawerUploadOpen = true;
    }

    private async Task FileUploaded(FileItem file)
    {
        if (_selectedProject is null)
        {
            return;
        }

        try
        {
            _isLoading = true;

            await Sender.Send(new UploadProjectFileCommand
            {
                Id = _selectedProject.Id,
                File = file
            });

            Snackbar.AddSuccess(SuccessMessageFor.Action($"{ProjectsDisplayTextFor.Project} File", file.FileName, CommonDisplayTextFor.Upload));

            await LoadItem();
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

    private async Task DownloadFile(ProjectFileItem item)
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

    private async Task DeleteProjectFile(ProjectFileItem item)
    {
        ClearException();

        var fileName = item.FileStorageName;
        var dialogResult = await DialogService.ShowMessageBox("Delete File",
            $"Do you want to delete this {item.FileStorageName}?",
            yesText: CommonDisplayTextFor.Yes,
            noText: CommonDisplayTextFor.No,
            options: new DialogOptions { MaxWidth = MaxWidth.ExtraSmall });

        if (dialogResult is true)
        {
            try
            {
                _isLoading = true;

                await Sender.Send(new DeleteProjectFileCommand
                {
                    Id = item.Id
                });

                Snackbar.AddSuccess(SuccessMessageFor.Action($"{ProjectsDisplayTextFor.Project} File", fileName, CommonDisplayTextFor.Deleted));

                await LoadItem();
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

    private void Details(ProjectItem item)
    {
        NavigationManager.NavigateTo(MainPageProjectsRouteFor.Details(item.Id));
    }
}
