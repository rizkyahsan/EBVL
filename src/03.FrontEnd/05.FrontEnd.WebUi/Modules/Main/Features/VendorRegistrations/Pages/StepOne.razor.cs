using EBVL.Shared.Dto.Modules.Main.VendorRegistrations.PreRegistration;
using EBVL.FrontEnd.WebUi.Modules.Main.Features.VendorRegistrations.Components;
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
    private bool _sapFound;
    private CompanyProfileForm? _companyProfileForm;

    protected override async Task OnInitializedAsync()
    {
        await RegistrationState.RestoreAsync();

        var sapVendorNumber = SapVendorNumber;
        if (string.IsNullOrWhiteSpace(sapVendorNumber))
        {
            sapVendorNumber = RegistrationState.PreRegistration?.SapVendorNumber;
        }

        if (string.IsNullOrWhiteSpace(sapVendorNumber))
        {
            return;
        }

        if (RegistrationState.PreRegistration?.SapVendorNumber == sapVendorNumber)
        {
            _model = RegistrationState.PreRegistration;
        }
        else
        {
            _model.SapVendorNumber = sapVendorNumber;
        }

        _sapFound = true;
    }

    private void BackToGuidance()
    {
        NavigationManager.NavigateTo(VendorRegistrationRouteFor.Index);
    }

    private void SapFound(string sapVendorNumber)
    {
        if (!string.Equals(_model.SapVendorNumber, sapVendorNumber, StringComparison.Ordinal))
        {
            _model = new PreRegistrationRequest
            {
                SapVendorNumber = sapVendorNumber
            };
        }

        _sapFound = true;
    }

    private async Task ContinueRegistration(PreRegistrationRequest model)
    {
        await RegistrationState.CompleteStepOneAsync(model);
        NavigationManager.NavigateTo(VendorRegistrationRouteFor.StepTwo);
    }

    private Task SubmitCompanyProfile()
    {
        return _companyProfileForm?.SubmitAsync() ?? Task.CompletedTask;
    }
}
