using EBVL.FrontEnd.Logics.Modules.MasterData.Questionnaires;

namespace EBVL.FrontEnd.WebUi.Modules.MasterData.Features.Questionnaires.Components;

public partial class DialogAdd
{
    private MudForm _form = default!;
    private string _businessProcess = string.Empty;
    private string _section = string.Empty;
    private VendorCompanyStatusType? _vendorType = VendorCompanyStatusType.Manufacture;
    private bool _isActive = true;

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
                VendorType = _vendorType,
                Section = _section,
                IsActive = _isActive
            };
            var created = await Sender.Send(command);
            Snackbar.AddSuccess("Questionnaire section added.");
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

    private static string Format(VendorCompanyStatusType value)
    {
        return value switch
        {
            VendorCompanyStatusType.Manufacture => "Vendor",
            VendorCompanyStatusType.SoleDistributorAgent => "Sole Agent",
            VendorCompanyStatusType.AuthorizedAgent => "Representative Office",
            _ => "Representative Office"
        };
    }
}
