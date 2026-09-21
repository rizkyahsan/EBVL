using EBVL.FrontEnd.WebUi.Modules.MasterData.Features.Questionnaires.Models;

namespace EBVL.FrontEnd.WebUi.Modules.MasterData.Features.Questionnaires.Components;

#pragma warning disable IDE0044
public partial class DialogSection
{
    [Parameter, EditorRequired] public required SectionModel Model { get; set; }
    [Parameter, EditorRequired] public required EventCallback<SectionModel> OnSubmit { get; set; }

    private MudForm _form = default!;

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
            if (_form.IsValid)
            {
                Model.Code = string.IsNullOrWhiteSpace(Model.Code) ? Code(Model.Title) : Model.Code;
                await OnSubmit.InvokeAsync(Model);
                Dialog.Close(DialogResult.Ok(Model));
            }
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

    private static string Code(string value)
    {
        return string.Join(
            '_',
            value.Trim().ToUpperInvariant().Split([' ', '-', '/'], StringSplitOptions.RemoveEmptyEntries));
    }

    private static string Vendor(VendorCompanyStatusType value)
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
