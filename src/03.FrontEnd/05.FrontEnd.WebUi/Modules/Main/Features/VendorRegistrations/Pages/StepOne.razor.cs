using EBVL.Shared.Dto.Modules.Main.VendorRegistrations.PreRegistration;
using EBVL.Shared.Dto.Modules.Main.VendorRegistrations.SapVendor;
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

    private readonly SapVendorRequest _sapModel = new();
    private PreRegistrationRequest _model = new();
    private bool _sapExpanded = true;
    private bool _profileExpanded;
    private bool _sapFound;

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

        _sapModel.SapVendorNumber = sapVendorNumber;

        if (RegistrationState.PreRegistration?.SapVendorNumber == sapVendorNumber)
        {
            _model = RegistrationState.PreRegistration;
        }
        else
        {
            _model.SapVendorNumber = sapVendorNumber;
        }

        ShowCompanyProfile();
    }

    private void BackToGuidance()
    {
        NavigationManager.NavigateTo(VendorRegistrationRouteFor.Index);
    }

    private void FindSapVendor(SapVendorRequest model)
    {
        if (_model.SapVendorNumber != model.SapVendorNumber)
        {
            _model = new PreRegistrationRequest
            {
                SapVendorNumber = model.SapVendorNumber
            };
        }

        ShowCompanyProfile();
    }

    private void ShowCompanyProfile()
    {
        _sapFound = true;
        _sapExpanded = true;
        _profileExpanded = true;
    }

    private void SetSapExpanded(bool expanded)
    {
        _sapExpanded = expanded;
    }

    private void SetProfileExpanded(bool expanded)
    {
        _profileExpanded = expanded;
    }

    private async Task ContinueRegistration(PreRegistrationRequest model)
    {
        await RegistrationState.CompleteStepOneAsync(model);
        NavigationManager.NavigateTo(VendorRegistrationRouteFor.StepTwo);
    }
}
