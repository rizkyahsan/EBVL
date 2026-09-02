using EBVL.FrontEnd.WebUi.Modules.Main.Features.VendorRegistrations.Services;
using VendorRegistrationRouteFor = EBVL.FrontEnd.WebUi.Modules.Main.Features.VendorRegistrations.Statics.RouteFor;

namespace EBVL.FrontEnd.WebUi.Modules.Main.Features.VendorRegistrations.Pages;

public partial class SuccessRegister
{
    [Inject]
    public required NavigationManager NavigationManager { get; init; }

    [Inject]
    public required VendorRegistrationState RegistrationState { get; init; }

    private bool _isRestoring = true;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender)
        {
            return;
        }

        await RegistrationState.RestoreAsync();
        if (!RegistrationState.IsEmailVerified)
        {
            NavigationManager.NavigateTo(VendorRegistrationRouteFor.EmailVerification);
            return;
        }

        _isRestoring = false;
        await InvokeAsync(StateHasChanged);
    }
}
