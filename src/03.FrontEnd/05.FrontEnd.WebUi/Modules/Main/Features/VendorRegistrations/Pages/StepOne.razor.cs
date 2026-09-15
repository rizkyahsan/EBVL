using EBVL.FrontEnd.Logics.Modules.Main.VendorRegistrations.Questionnaires;
using EBVL.FrontEnd.WebUi.Modules.Main.Features.VendorRegistrations.Components;
using EBVL.FrontEnd.WebUi.Modules.Main.Features.VendorRegistrations.Services;
using EBVL.Shared.Dto.Modules.Main.VendorRegistrations.PreRegistration;
using EBVL.Shared.Dto.Modules.Main.VendorRegistrations.Questionnaires;
using MediatR;
using VendorRegistrationRouteFor = EBVL.FrontEnd.WebUi.Modules.Main.Features.VendorRegistrations.Statics.RouteFor;

namespace EBVL.FrontEnd.WebUi.Modules.Main.Features.VendorRegistrations.Pages;

public partial class StepOne
{
    #region Dependencies

    [Inject]
    public required NavigationManager NavigationManager { get; init; }

    [Inject]
    public required VendorRegistrationState RegistrationState { get; init; }
    [Inject] public required ISender Sender { get; init; }

    #endregion

    #region Parameters

    [Parameter]
    [SupplyParameterFromQuery]
    public string? SapVendorNumber { get; set; }

    #endregion

    #region Fields

    private PreRegistrationRequest _model = new();
    private bool _sapFound;
    private CompanyProfileForm? _companyProfileForm;

    #endregion

    #region Lifecycle

    protected override async Task OnInitializedAsync()
    {
        await RegistrationState.RestoreAsync();
        if (RegistrationState.RegistrationId is not null && !string.IsNullOrWhiteSpace(RegistrationState.ResumeToken))
        {
            _ = await RegistrationState.RestoreRuntimeAsync(Sender);
        }

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

    #endregion

    #region Private Methods

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
        QuestionnaireRuntimeResponse runtime;
        if (RegistrationState.RegistrationId is not null && !string.IsNullOrWhiteSpace(RegistrationState.ResumeToken) && RegistrationState.Runtime is not null)
        {
            runtime = await Sender.Send(new UpdateVendorRegistrationProfileCommand(RegistrationState.RegistrationId.Value,
                new(RegistrationState.ResumeToken, RegistrationState.Runtime.RowVersion, model)));
        }
        else
        {
            runtime = await Sender.Send(new StartQuestionnaireCommand(model));
        }

        await RegistrationState.SetRuntimeAsync(runtime);
        NavigationManager.NavigateTo(VendorRegistrationRouteFor.StepTwo);
    }

    private Task SubmitCompanyProfile()
    {
        return _companyProfileForm?.SubmitAsync() ?? Task.CompletedTask;
    }

    #endregion
}
