using EBVL.FrontEnd.Logics.Modules.MasterData.Questionnaires;

namespace EBVL.FrontEnd.WebUi.Modules.MasterData.Features.Questionnaires.Components;

public partial class DialogAdd
{
    private MudForm _form = default!;
    private string _businessProcess = string.Empty;
    private string _code = string.Empty;

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

            var command = new AddQuestionnaireCommand
            {
                BusinessProcess = _businessProcess,
                Code = _code,
                IsActive = false
            };
            var created = await Sender.Send(command);
            Snackbar.AddSuccess("Questionnaire business process added.");
            Dialog.Close(DialogResult.Ok(created.Item));
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
