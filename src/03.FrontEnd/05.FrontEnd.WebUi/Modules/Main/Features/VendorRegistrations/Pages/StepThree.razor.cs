using EBVL.FrontEnd.Logics.Modules.Main.VendorRegistrations.Questionnaires;
using EBVL.FrontEnd.WebUi.Modules.Main.Features.VendorRegistrations.Services;
using EBVL.Shared.Dto.Modules.Main.VendorRegistrations.Questionnaires;
using MediatR;
using VendorRegistrationRouteFor = EBVL.FrontEnd.WebUi.Modules.Main.Features.VendorRegistrations.Statics.RouteFor;

namespace EBVL.FrontEnd.WebUi.Modules.Main.Features.VendorRegistrations.Pages;

public partial class StepThree
{
    #region Dependencies

    [Inject]
    public required NavigationManager NavigationManager { get; init; }

    [Inject]
    public required VendorRegistrationState RegistrationState { get; init; }
    [Inject] public required ISender Sender { get; init; }
    [Inject] public required ISnackbar Snackbar { get; init; }

    #endregion

    #region Fields

    private IReadOnlyList<RuntimeSectionItem> _sections = [];
    private bool _isRestoring = true;

    #endregion

    #region Lifecycle

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender)
        {
            return;
        }

        if (!await RegistrationState.RestoreRuntimeAsync(Sender) || RegistrationState.Runtime is null || !RegistrationState.Runtime.IsDocumentEvidenceComplete)
        {
            NavigationManager.NavigateTo(VendorRegistrationRouteFor.StepTwo);
            return;
        }

        _sections = RegistrationState.Runtime.Sections;

        _isRestoring = false;
        await InvokeAsync(StateHasChanged);
    }

    #endregion

    #region Private Methods

    private async Task ContinueRegistration(IReadOnlyList<QuestionnaireAnswerValue> answers)
    {
        try
        {
            var latestRuntime = await Sender.Send(new GetQuestionnaireRuntimeQuery(RegistrationState.RegistrationId!.Value, RegistrationState.ResumeToken!));
            var runtime = await Sender.Send(new SaveQuestionnaireAnswersCommand(RegistrationState.RegistrationId!.Value, new(RegistrationState.ResumeToken!, latestRuntime.RowVersion, answers)));
            await RegistrationState.SetRuntimeAsync(runtime);
            NavigationManager.NavigateTo(VendorRegistrationRouteFor.Review);
        }
        catch (Exception exception)
        {
            Snackbar.AddError(exception.Message);
        }
    }

    private void BackToGuidance()
    {
        NavigationManager.NavigateTo(VendorRegistrationRouteFor.Index);
    }

    private void BackToStepTwo()
    {
        NavigationManager.NavigateTo(VendorRegistrationRouteFor.StepTwo);
    }

    private async Task UploadFile(RuntimeQuestionItem question, IBrowserFile file)
    {
        await using var stream = file.OpenReadStream(50L * 1024 * 1024);
        using var memory = new MemoryStream();
        await stream.CopyToAsync(memory);
        _ = await Sender.Send(new UploadQuestionnaireFileCommand(RegistrationState.RegistrationId!.Value, question.Id, RegistrationState.ResumeToken!, file.Name, memory.ToArray()));
        await RefreshRuntime();
    }

    private async Task RemoveFile(RuntimeFileItem file)
    {
        await Sender.Send(new DeleteQuestionnaireFileCommand(RegistrationState.RegistrationId!.Value, file.Id, RegistrationState.ResumeToken!));
        await RefreshRuntime();
    }

    private async Task RefreshRuntime()
    {
        var runtime = await Sender.Send(new GetQuestionnaireRuntimeQuery(RegistrationState.RegistrationId!.Value, RegistrationState.ResumeToken!));
        await RegistrationState.SetRuntimeAsync(runtime);
        _sections = runtime.Sections;
    }

    #endregion
}
