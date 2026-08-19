using EBVL.Shared.Dto.Common.FileStorages;

namespace EBVL.FrontEnd.WebUi.Modules.Main.Features.Projects.Components;

public partial class DrawerUploadAttachment
{
    [Parameter, EditorRequired]
    public required string Name { get; init; }

    [Parameter, EditorRequired]
    public required string Desc { get; init; }

    [Parameter, EditorRequired]
    public EventCallback<FileItem> OnFileUploaded { get; set; }

    private IBrowserFile? _fileUploaded;

    private void FileUpdated(IBrowserFile? file)
    {
        _fileUploaded = file;
    }

    private async Task Submit()
    {
        ClearException();

        try
        {
            _isLoading = true;

            if (_fileUploaded is null)
            {
                throw new InvalidOperationException($"Please choose a file.");
            }

            var fileItem = new FileItem
            {
                FileContent = await _fileUploaded.ToBytesAsync(DocumentsMaximumValueFor.FileSize),
                FileName = _fileUploaded.Name,
                ContentType = _fileUploaded.ContentType
            };

            Snackbar.AddSuccess(SuccessMessageFor.Added(ProjectAttachmentsDisplayTextFor.ProjectAttachment));

            await OnFileUploaded.InvokeAsync(fileItem);
            await IsOpenChanged.InvokeAsync(false);
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
