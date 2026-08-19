using EBVL.FrontEnd.Logics.Modules.Examples.Documents.AddDocument;
using EBVL.Shared.Dto.Common.FileStorages;

namespace EBVL.FrontEnd.WebUi.Modules.Examples.Features.Documents.Components;

public partial class DialogAdd
{
    [Parameter]
    public required DialogAddModel Model { get; init; }

    private void FileUpdated(IBrowserFile? file)
    {
        Model.File = file;
    }

    private async Task Submit()
    {
        try
        {
            _isLoading = true;

            if (Model.File is null)
            {
                throw new InvalidOperationException($"Please choose a file.");
            }

            var command = new AddDocumentCommand
            {
                Description = Model.Description,
                File = new FileItem
                {
                    FileContent = await Model.File.ToBytesAsync(DocumentsMaximumValueFor.FileSize),
                    FileName = Model.File.Name,
                    ContentType = Model.File.ContentType
                }
            };

            var response = await Sender.Send(command);

            Snackbar.AddSuccess(SuccessMessageFor.Added(DocumentsDisplayTextFor.Document));

            Dialog.Close(DialogResult.Ok(response.Item.Id));
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

public sealed record DialogAddModel
{
    public required string Description { get; set; }
    public IBrowserFile? File { get; set; }
}
