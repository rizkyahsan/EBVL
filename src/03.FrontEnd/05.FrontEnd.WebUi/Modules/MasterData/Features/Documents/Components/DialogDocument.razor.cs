using EBVL.FrontEnd.Logics.Modules.MasterData.Documents;
using EBVL.Shared.Dto.Modules.MasterData.Documents;

namespace EBVL.FrontEnd.WebUi.Modules.MasterData.Features.Documents.Components;

public partial class DialogDocument
{
    [Parameter]
    public DocumentListItem? Document { get; set; }

    private MudForm _form = default!;
    private string _businessProcess = string.Empty;
    private string _name = string.Empty;
    private int _maxSizeMb = 1;
    private bool _isMandatory;
    private bool _isActive = true;

    protected override void OnInitialized()
    {
        if (Document is null)
        {
            return;
        }

        _businessProcess = Document.BusinessProcess;
        _name = Document.Name;
        _maxSizeMb = Document.MaxSizeMb;
        _isMandatory = Document.IsMandatory;
        _isActive = Document.IsActive;
    }

    private async Task Submit()
    {
        if (_isLoading)
        {
            return;
        }

        try
        {
            _isLoading = true;
            ClearException();
            await _form.Validate();

            if (!_form.IsValid)
            {
                return;
            }

            GetDocumentResponse response;
            if (Document is null)
            {
                response = await Sender.Send(new AddDocumentCommand
                {
                    BusinessProcess = _businessProcess,
                    Name = _name,
                    MaxSizeMb = _maxSizeMb,
                    IsMandatory = _isMandatory,
                    IsActive = _isActive
                });
            }
            else
            {
                var request = new UpdateDocumentRequest
                {
                    BusinessProcess = _businessProcess,
                    Name = _name,
                    MaxSizeMb = _maxSizeMb,
                    IsMandatory = _isMandatory,
                    IsActive = _isActive,
                    RowVersion = Document.RowVersion
                };
                response = await Sender.Send(new UpdateDocumentCommand(Document.Id, request));
            }

            Snackbar.AddSuccess(Document is null ? "Document added." : "Document updated.");
            Dialog.Close(DialogResult.Ok(response.Item));
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
