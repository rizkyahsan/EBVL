using EBVL.FrontEnd.Logics.Modules.Main.VendorRegistrations.Questionnaires;
using EBVL.FrontEnd.WebUi.Modules.Main.Features.VendorRegistrations.Services;
using EBVL.Shared.Dto.Modules.Main.VendorRegistrations.Questionnaire;
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
            _sections = runtime.Sections;
            if (runtime.Sections.SelectMany(section => section.Questions).Any(question => question.IsRequired && !HasValue(question)))
            {
                Snackbar.AddWarning("Additional required questions are now available. Complete them before continuing.");
                return;
            }

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

    private static bool HasValue(RuntimeQuestionItem question)
    {
        var value = question.Answer;
        return question.Type switch
        {
            QuestionnaireQuestionType.ShortText or QuestionnaireQuestionType.LongText => !string.IsNullOrWhiteSpace(value?.TextValue),
            QuestionnaireQuestionType.Address => IsCompleteAddress(value?.AddressJson),
            QuestionnaireQuestionType.Integer => value?.IntegerValue is not null,
            QuestionnaireQuestionType.Decimal => value?.DecimalValue is not null,
            QuestionnaireQuestionType.Date => value?.DateValue is not null,
            QuestionnaireQuestionType.Boolean => value?.BooleanValue is not null,
            QuestionnaireQuestionType.SingleChoice or QuestionnaireQuestionType.MultipleChoice => value?.OptionIds?.Count > 0,
            QuestionnaireQuestionType.File => question.Files.Count > 0,
            _ => false
        };
    }

    private static bool IsCompleteAddress(string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return false;
        }

        try
        {
            var address = System.Text.Json.JsonSerializer.Deserialize<QuestionnaireAddressRequest>(json);
            return address is not null
                && !string.IsNullOrWhiteSpace(address.Country)
                && !string.IsNullOrWhiteSpace(address.Building)
                && !string.IsNullOrWhiteSpace(address.Street)
                && !string.IsNullOrWhiteSpace(address.Number)
                && !string.IsNullOrWhiteSpace(address.City)
                && !string.IsNullOrWhiteSpace(address.Phone)
                && !string.IsNullOrWhiteSpace(address.Fax)
                && !string.IsNullOrWhiteSpace(address.Email)
                && !string.IsNullOrWhiteSpace(address.Website);
        }
        catch (System.Text.Json.JsonException)
        {
            return false;
        }
    }

    #endregion
}
