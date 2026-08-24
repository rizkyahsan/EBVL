using EBVL.FrontEnd.WebUi.Modules.Main.Features.VendorRegistrations.Services;
using EBVL.Shared.Dto.Modules.Main.VendorRegistrations.Questionnaire;
using VendorRegistrationRouteFor = EBVL.FrontEnd.WebUi.Modules.Main.Features.VendorRegistrations.Statics.RouteFor;

namespace EBVL.FrontEnd.WebUi.Modules.Main.Features.VendorRegistrations.Pages;

public partial class StepTwo
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

        if (!RegistrationState.IsStepOneCompleted || RegistrationState.PreRegistration is null)
        {
            NavigationManager.NavigateTo(VendorRegistrationRouteFor.Sap);
            return;
        }

        _model = RegistrationState.Questionnaire ?? new QuestionnaireRequest
        {
            SapVendorNumber = RegistrationState.PreRegistration.SapVendorNumber
        };

        _isRestoring = false;
        await InvokeAsync(StateHasChanged);
    }

    private void NavigateToStepOne()
    {
        var sapVendorNumber = RegistrationState.PreRegistration?.SapVendorNumber;
        NavigationManager.NavigateTo(string.IsNullOrWhiteSpace(sapVendorNumber)
            ? VendorRegistrationRouteFor.Sap
            : VendorRegistrationRouteFor.StepOneWith(sapVendorNumber));
    }

    private async Task ContinueRegistration(QuestionnaireRequest model)
    {
        await RegistrationState.CompleteStepTwoAsync(model);
        NavigationManager.NavigateTo(VendorRegistrationRouteFor.StepThree);
    }

    private async Task RejectQuestionnaire(QuestionnaireRequest model)
    {
        await RegistrationState.UpdateStepTwoAsync(model);
        NavigateToStepOne();
    }

    private Task PersistProgress(QuestionnaireRequest model)
    {
        return RegistrationState.UpdateStepTwoAsync(model);
    }
}
