using EBVL.FrontEnd.WebUi.Modules.Main.Features.VendorRegistrations.Services;
using EBVL.Shared.Dto.Modules.Main.VendorRegistrations.DocumentEvidence;
using VendorRegistrationRouteFor = EBVL.FrontEnd.WebUi.Modules.Main.Features.VendorRegistrations.Statics.RouteFor;

namespace EBVL.FrontEnd.WebUi.Modules.Main.Features.VendorRegistrations.Pages;

public partial class StepTwo
{
    [Inject]
    public required NavigationManager NavigationManager { get; init; }

    [Inject]
    public required VendorRegistrationState RegistrationState { get; init; }

    private DocumentEvidenceRequest _model = new();
    private bool _isRestoring = true;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender)
        {
            return;
        }

        await RegistrationState.RestoreAsync();

        if (!RegistrationState.IsStepOneCompleted || RegistrationState.PreRegistration is null)
        {
            NavigationManager.NavigateTo(VendorRegistrationRouteFor.StepOne);
            return;
        }

        _model = RegistrationState.DocumentEvidence ?? new DocumentEvidenceRequest
        {
            SapVendorNumber = RegistrationState.PreRegistration.SapVendorNumber
        };

        _isRestoring = false;
        await InvokeAsync(StateHasChanged);
    }

    private void BackToGuidance()
    {
        NavigationManager.NavigateTo(VendorRegistrationRouteFor.Index);
    }

    private void BackToStepOne()
    {
        var sapVendorNumber = RegistrationState.PreRegistration?.SapVendorNumber;
        NavigationManager.NavigateTo(string.IsNullOrWhiteSpace(sapVendorNumber)
            ? VendorRegistrationRouteFor.StepOne
            : VendorRegistrationRouteFor.StepOneWith(sapVendorNumber));
    }

    private async Task SelectFile(string key, IBrowserFile file)
    {
        await RegistrationState.SetDocumentAsync(key, file);
        _model = RegistrationState.DocumentEvidence!;
    }

    private async Task RemoveFile(string key)
    {
        await RegistrationState.RemoveDocumentAsync(key);
        _model = RegistrationState.DocumentEvidence!;
    }

    private async Task ContinueRegistration(DocumentEvidenceRequest model)
    {
        await RegistrationState.CompleteStepTwoAsync(model);
        NavigationManager.NavigateTo(VendorRegistrationRouteFor.StepThree);
    }
}
