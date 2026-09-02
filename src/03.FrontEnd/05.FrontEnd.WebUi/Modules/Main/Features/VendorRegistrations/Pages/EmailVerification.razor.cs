using EBVL.FrontEnd.WebUi.Modules.Main.Features.VendorRegistrations.Services;
using VendorRegistrationRouteFor = EBVL.FrontEnd.WebUi.Modules.Main.Features.VendorRegistrations.Statics.RouteFor;

namespace EBVL.FrontEnd.WebUi.Modules.Main.Features.VendorRegistrations.Pages;

public partial class EmailVerification
{
    [Inject]
    public required NavigationManager NavigationManager { get; init; }

    [Inject]
    public required VendorRegistrationState RegistrationState { get; init; }

    private bool _isRestoring = true;
    private string _email = string.Empty;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender)
        {
            return;
        }

        await RegistrationState.RestoreAsync();
        if (!RegistrationState.IsVerificationSent || RegistrationState.PreRegistration is null)
        {
            NavigationManager.NavigateTo(VendorRegistrationRouteFor.Review);
            return;
        }

        _email = RegistrationState.PreRegistration.CompanyEmail;
        _isRestoring = false;
        await InvokeAsync(StateHasChanged);
    }

    private async Task VerifyEmail()
    {
        await RegistrationState.MarkEmailVerifiedAsync();
        NavigationManager.NavigateTo(VendorRegistrationRouteFor.Success);
    }
}
