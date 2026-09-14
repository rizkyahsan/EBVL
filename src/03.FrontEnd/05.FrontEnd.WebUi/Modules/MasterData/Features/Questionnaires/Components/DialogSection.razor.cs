using EBVL.FrontEnd.WebUi.Modules.MasterData.Features.Questionnaires.Models;

namespace EBVL.FrontEnd.WebUi.Modules.MasterData.Features.Questionnaires.Components;

public partial class DialogSection
{
    [Parameter, EditorRequired] public required SectionModel Model { get; set; }

    private MudForm _form = default!;

    private async Task Submit()
    {
        await _form.Validate();

        if (_form.IsValid)
        {
            Dialog.Close(DialogResult.Ok(Model));
        }
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
