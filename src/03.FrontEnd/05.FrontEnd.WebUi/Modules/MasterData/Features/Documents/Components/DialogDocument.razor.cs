using EBVL.FrontEnd.Logics.Modules.MasterData.Documents;
using EBVL.Shared.Dto.Modules.MasterData.Documents;

namespace EBVL.FrontEnd.WebUi.Modules.MasterData.Features.Documents.Components;

public partial class DialogDocument
{
    [Parameter]
    public DocumentListItem? Document { get; set; }
    [Parameter]
    public required DocumentRequirementSetItem RequirementSet { get; set; }

    private MudForm _form = default!;
    private string _businessProcess = string.Empty;
    private string _name = string.Empty;
    private int _maxSizeMb = 1;
    private int _order = 1;
    private bool _isMandatory;
    private bool _isActive = true;

    protected override void OnInitialized()
    {
        _businessProcess = RequirementSet.BusinessProcess;
        _order = RequirementSet.Requirements.Count + 1;
        if (Document is null)
        {
            return;
        }

        _businessProcess = Document.BusinessProcess;
        _name = Document.Name;
        _maxSizeMb = Document.MaxSizeMb;
        _order = Document.Order;
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
                response = await Sender.Send(new AddDocumentCommand(RequirementSet.Id, new AddDocumentRequest
                {
                    BusinessProcess = _businessProcess,
                    Name = _name,
                    MaxSizeMb = _maxSizeMb,
                    Order = _order,
                    IsMandatory = _isMandatory,
                    IsActive = _isActive,
                    RowVersion = RequirementSet.RowVersion
                }));
            }
            else
            {
                var request = new UpdateDocumentRequest
                {
                    BusinessProcess = _businessProcess,
                    Name = _name,
                    MaxSizeMb = _maxSizeMb,
                    Order = _order,
                    IsMandatory = _isMandatory,
                    IsActive = _isActive,
                    RowVersion = RequirementSet.RowVersion
                };
                response = await Sender.Send(new UpdateDocumentCommand(RequirementSet.Id, Document.Id, request));
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
