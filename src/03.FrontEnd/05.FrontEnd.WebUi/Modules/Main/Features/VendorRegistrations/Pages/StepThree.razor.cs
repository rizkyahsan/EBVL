using EBVL.FrontEnd.WebUi.Modules.Main.Features.VendorRegistrations.Services;
using EBVL.Shared.Dto.Modules.Main.VendorRegistrations.Questionnaire;
using VendorRegistrationRouteFor = EBVL.FrontEnd.WebUi.Modules.Main.Features.VendorRegistrations.Statics.RouteFor;

namespace EBVL.FrontEnd.WebUi.Modules.Main.Features.VendorRegistrations.Pages;

public partial class StepThree
{
    [Inject]
    public required NavigationManager NavigationManager { get; init; }

    [Inject]
    public required VendorRegistrationState RegistrationState { get; init; }

    private QuestionnaireRequest _model = new();
    private bool _isRestoring = true;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender)
        {
            return;
        }

        await RegistrationState.RestoreAsync();

        if (!RegistrationState.IsStepTwoCompleted || RegistrationState.PreRegistration is null)
        {
            NavigationManager.NavigateTo(VendorRegistrationRouteFor.StepTwo);
            return;
        }

        _model = RegistrationState.Questionnaire ?? new QuestionnaireRequest
        {
            SapVendorNumber = RegistrationState.PreRegistration.SapVendorNumber
        };

        _isRestoring = false;
        await InvokeAsync(StateHasChanged);
    }

    private async Task ContinueRegistration(QuestionnaireRequest model)
    {
        await RegistrationState.CompleteStepThreeAsync(model);
        NavigationManager.NavigateTo(VendorRegistrationRouteFor.Review);
    }

    private void BackToGuidance()
    {
        NavigationManager.NavigateTo(VendorRegistrationRouteFor.Index);
    }

    private void BackToStepTwo()
    {
        NavigationManager.NavigateTo(VendorRegistrationRouteFor.StepTwo);
    }

    private Task PersistProgress(QuestionnaireRequest model)
    {
        return RegistrationState.UpdateStepThreeAsync(model);
    }
}
