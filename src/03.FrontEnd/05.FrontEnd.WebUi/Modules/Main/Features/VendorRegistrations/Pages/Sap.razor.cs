using EBVL.Shared.Dto.Modules.Main.VendorRegistrations.SapVendor;
using VendorRegistrationRouteFor = EBVL.FrontEnd.WebUi.Modules.Main.Features.VendorRegistrations.Statics.RouteFor;

namespace EBVL.FrontEnd.WebUi.Modules.Main.Features.VendorRegistrations.Pages;

public partial class Sap
{
    [Inject]
    public required NavigationManager NavigationManager { get; init; }

    private readonly SapVendorRequest _model = new();

    private void BackToWelcome()
    {
        NavigationManager.NavigateTo(VendorRegistrationRouteFor.Index);
    }

    private void OpenStepOne(SapVendorRequest model)
    {
        NavigationManager.NavigateTo(VendorRegistrationRouteFor.StepOneWith(model.SapVendorNumber));
    }
}
