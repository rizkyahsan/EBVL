using EBVL.FrontEnd.Logics.Modules.Examples.Documents.UpdateDocument;

namespace EBVL.FrontEnd.WebUi.Modules.Examples.Features.Documents.Components;

public partial class DialogEdit
{
    [Parameter]
    public required DialogEditModel Model { get; init; }

    private async Task Submit()
    {
        try
        {
            _isLoading = true;

            ClearException();

            var command = new UpdateDocumentCommand
            {
                DocumentId = Model.DocumentId,
                Description = Model.Description
            };

            await Sender.Send(command);

            Snackbar.AddSuccess(SuccessMessageFor.Updated(DocumentsDisplayTextFor.Document, Model.FileName));

            Dialog.Close();
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

public sealed record DialogEditModel
{
    public required Guid DocumentId { get; init; }
    public required string FileName { get; init; }
    public required string Description { get; set; }
}
