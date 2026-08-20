using EBVL.Shared.Dto.Modules.Main.VendorRegistrations.PreRegistration;
using VendorRegistrationRouteFor = EBVL.FrontEnd.WebUi.Modules.Main.Features.VendorRegistrations.Statics.RouteFor;

namespace EBVL.FrontEnd.WebUi.Modules.Main.Features.VendorRegistrations.Pages;

public partial class StepOne
{
    [Inject]
    public required NavigationManager NavigationManager { get; init; }

    [Parameter]
    [SupplyParameterFromQuery]
    public string? SapVendorNumber { get; set; }

    private readonly PreRegistrationRequest _model = new();

    protected override void OnParametersSet()
    {
        if (string.IsNullOrWhiteSpace(SapVendorNumber))
        {
            NavigationManager.NavigateTo(VendorRegistrationRouteFor.Sap);
            return;
        }

        _model.SapVendorNumber = SapVendorNumber;
    }

    private void BackToSap()
    {
        NavigationManager.NavigateTo(VendorRegistrationRouteFor.Sap);
    }

    private Task ContinueRegistration(PreRegistrationRequest model)
    {
        // Step 2 will persist or forward this DTO once its contract is defined.
        return Task.CompletedTask;
    }
}
