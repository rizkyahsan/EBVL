using EBVL.Shared.Dto.Modules.Main.VendorRegistrations.Questionnaire;
using EBVL.Shared.Statics.VendorRegistrations;

namespace EBVL.FrontEnd.WebUi.Modules.Main.Features.VendorRegistrations.Components;

public partial class QuestionnaireForm
{
    [Parameter]
    public required QuestionnaireRequest Model { get; init; }

    [Parameter]
    public required EventCallback OnBack { get; init; }

    [Parameter]
    public required EventCallback<QuestionnaireRequest> OnNext { get; init; }

    [Parameter]
    public EventCallback<QuestionnaireRequest> OnChanged { get; init; }

    private MudForm _form = default!;
    private readonly QuestionnaireRequestValidator _validator = new();
    private string? _validationError;

    private static IReadOnlyList<VendorQuestionDefinition> VendorRepresentativeQuestions => QuestionsFor(QuestionnaireFor.VendorRepresentativeOffice, [14, 17, 15, 18, 16, 19]);
    private static IReadOnlyList<VendorQuestionDefinition> SoleAgentQuestions => QuestionsFor(QuestionnaireFor.SoleAgent, [20, 24, 21, 25, 22, 26, 23]);
    private static IReadOnlyList<VendorQuestionDefinition> ProductQualityGeneralQuestions => QuestionsFor(QuestionnaireFor.ProductQualityGeneral, [27, 30, 28, 31, 29, 32]);
    private static IReadOnlyList<VendorQuestionDefinition> ProductQualitySpecificQuestions => QuestionsFor(QuestionnaireFor.ProductQualitySpecific, [33, 39, 34, 40, 35, 41, 36, 42, 37, 43, 38, 44]);
    private static IReadOnlyList<VendorQuestionDefinition> ProductPositioningQuestions => QuestionsFor(QuestionnaireFor.ProductPositioning, [45, 48, 46, 49, 47, 50]);

    private static IReadOnlyList<VendorQuestionDefinition> QuestionsFor(string section, IReadOnlyList<int> order)
    {
        var positions = order
            .Select((number, index) => new { number, index })
            .ToDictionary(item => item.number, item => item.index);

        return
        [
            .. QuestionnaireFor.All
                .Where(question => question.Section == section)
                .OrderBy(question => positions[question.Number])
        ];
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
        await _form.Validate();
        var validationResult = await _validator.ValidateAsync(Model);

        if (!_form.IsValid || !validationResult.IsValid)
        {
            _validationError = validationResult.Errors.FirstOrDefault()?.ErrorMessage ?? "Periksa kembali field wajib.";
            return;
        }

        Model.IsSubmitQuestionnaire = true;
        await OnNext.InvokeAsync(Model);
    }
}
