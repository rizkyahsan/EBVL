using VendorRegistrationRouteFor = EBVL.FrontEnd.WebUi.Modules.Main.Features.VendorRegistrations.Statics.RouteFor;

namespace EBVL.FrontEnd.WebUi.Modules.Main.Features.VendorRegistrations.Pages;

public partial class Index
{
    [Inject]
    public required NavigationManager NavigationManager { get; init; }

    private void BackToLogin()
    {
        NavigationManager.NavigateTo(AuthenticationRouteFor.LocalLoginPage());
    }

    private void OpenSapForm()
    {
        NavigationManager.NavigateTo(VendorRegistrationRouteFor.Sap);
    }
}
