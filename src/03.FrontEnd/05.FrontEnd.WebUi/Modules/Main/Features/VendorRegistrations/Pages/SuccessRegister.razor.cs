using EBVL.FrontEnd.WebUi.Modules.Main.Features.VendorRegistrations.Services;
using MediatR;
using VendorRegistrationRouteFor = EBVL.FrontEnd.WebUi.Modules.Main.Features.VendorRegistrations.Statics.RouteFor;

namespace EBVL.FrontEnd.WebUi.Modules.Main.Features.VendorRegistrations.Pages;

public partial class SuccessRegister
{
    [Inject]
    public required NavigationManager NavigationManager { get; init; }

    [Inject]
    public required VendorRegistrationState RegistrationState { get; init; }
    [Inject] public required ISender Sender { get; init; }

    private bool _isRestoring = true;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender)
        {
            return;
        }

        if (!await RegistrationState.RestoreRuntimeAsync(Sender) || RegistrationState.Runtime?.Status != VendorRegistrationStatus.Submitted)
        {
            NavigationManager.NavigateTo(VendorRegistrationRouteFor.EmailVerification);
            return;
        }

        _isRestoring = false;
        await InvokeAsync(StateHasChanged);
    }
}
