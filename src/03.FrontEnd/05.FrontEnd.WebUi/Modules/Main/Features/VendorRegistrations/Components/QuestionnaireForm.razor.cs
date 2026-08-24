using EBVL.Shared.Dto.Modules.Main.VendorRegistrations.Questionnaire;
using EBVL.Shared.Statics.VendorRegistrations;

namespace EBVL.FrontEnd.WebUi.Modules.Main.Features.VendorRegistrations.Components;

public partial class QuestionnaireForm
{
    [Parameter]
    public required QuestionnaireRequest Model { get; init; }

    [Parameter]
    public required EventCallback<QuestionnaireRequest> OnReject { get; init; }

    [Parameter]
    public required EventCallback<QuestionnaireRequest> OnNext { get; init; }

    [Parameter]
    public EventCallback<QuestionnaireRequest> OnChanged { get; init; }

    private MudForm _form = default!;
    private readonly QuestionnaireRequestValidator _validator = new();
    private string? _validationError;

    private static IReadOnlyList<VendorQuestionDefinition> GeneralInformationQuestions => QuestionsFor(QuestionnaireFor.GeneralInformation);
    private static IReadOnlyList<VendorQuestionDefinition> VendorRepresentativeQuestions => QuestionsFor(QuestionnaireFor.VendorRepresentativeOffice);
    private static IReadOnlyList<VendorQuestionDefinition> SoleAgentQuestions => QuestionsFor(QuestionnaireFor.SoleAgent);
    private static IReadOnlyList<VendorQuestionDefinition> ProductQualityGeneralQuestions => QuestionsFor(QuestionnaireFor.ProductQualityGeneral);
    private static IReadOnlyList<VendorQuestionDefinition> ProductQualitySpecificQuestions => QuestionsFor(QuestionnaireFor.ProductQualitySpecific);
    private static IReadOnlyList<VendorQuestionDefinition> ProductPositioningQuestions => QuestionsFor(QuestionnaireFor.ProductPositioning);

    private static IReadOnlyList<VendorQuestionDefinition> QuestionsFor(string section)
    {
        return [.. QuestionnaireFor.All.Where(question => question.Section == section)];
    }

    private Task NotifyChanged()
    {
        return OnChanged.InvokeAsync(Model);
    }

    private async Task SoleAgentChanged(bool value)
    {
        Model.IsNotSoleAgent = value;
        await NotifyChanged();
    }

    private async Task Submit()
    {
        _validationError = null;
        Model.IsSubmitQuestionnaire = true;
        await _form.Validate();
        var validationResult = await _validator.ValidateAsync(Model);

        if (!_form.IsValid || !validationResult.IsValid)
        {
            _validationError = validationResult.Errors.FirstOrDefault()?.ErrorMessage ?? "Periksa kembali field wajib.";
            return;
        }

        await OnNext.InvokeAsync(Model);
    }

    private async Task Reject()
    {
        Model.IsSubmitQuestionnaire = false;
        await OnChanged.InvokeAsync(Model);
        await OnReject.InvokeAsync(Model);
    }
}
