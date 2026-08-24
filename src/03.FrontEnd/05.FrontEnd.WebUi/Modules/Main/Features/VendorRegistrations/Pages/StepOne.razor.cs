using EBVL.Shared.Dto.Modules.Main.VendorRegistrations.PreRegistration;
using EBVL.FrontEnd.WebUi.Modules.Main.Features.VendorRegistrations.Services;
using VendorRegistrationRouteFor = EBVL.FrontEnd.WebUi.Modules.Main.Features.VendorRegistrations.Statics.RouteFor;

namespace EBVL.FrontEnd.WebUi.Modules.Main.Features.VendorRegistrations.Pages;

public partial class StepOne
{
    [Inject]
    public required NavigationManager NavigationManager { get; init; }

    [Inject]
    public required VendorRegistrationState RegistrationState { get; init; }

    [Parameter]
    [SupplyParameterFromQuery]
    public string? SapVendorNumber { get; set; }

    private PreRegistrationRequest _model = new();

    protected override void OnParametersSet()
    {
        if (string.IsNullOrWhiteSpace(SapVendorNumber))
        {
            NavigationManager.NavigateTo(VendorRegistrationRouteFor.Sap);
            return;
        }

        if (RegistrationState.PreRegistration?.SapVendorNumber == SapVendorNumber)
        {
            _model = RegistrationState.PreRegistration;
        }
        else
        {
            _model.SapVendorNumber = SapVendorNumber;
        }
    }

    private void BackToSap()
    {
        NavigationManager.NavigateTo(VendorRegistrationRouteFor.Sap);
    }

    private async Task ContinueRegistration(PreRegistrationRequest model)
    {
        await RegistrationState.CompleteStepOneAsync(model);
        NavigationManager.NavigateTo(VendorRegistrationRouteFor.StepTwo);
    }
}
