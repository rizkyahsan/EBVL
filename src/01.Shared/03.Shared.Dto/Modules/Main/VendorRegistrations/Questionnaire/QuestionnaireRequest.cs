namespace EBVL.Shared.Dto.Modules.Main.VendorRegistrations.Questionnaire;

public record QuestionnaireRequest
{
    public string SapVendorNumber { get; set; } = string.Empty;
    public bool IsNotSoleAgent { get; set; }
    public bool? IsSubmitQuestionnaire { get; set; }
    public QuestionnaireAddressRequest HeadquarterAddress { get; set; } = new();
    public QuestionnaireAddressRequest ManufacturingAddress { get; set; } = new();
    public QuestionnaireAddressRequest RepresentativeOfficeAddress { get; set; } = new();
    public QuestionnaireAddressRequest SoleAgentOfficeAddress { get; set; } = new();
    public List<QuestionnaireAnswerRequest> Answers { get; set; } =
    [
        .. EBVL.Shared.Statics.VendorRegistrations.QuestionnaireFor.All.Select(question => new QuestionnaireAnswerRequest
        {
            QuestionNumber = question.Number
        })
    ];
}

public sealed class QuestionnaireRequestValidator : AbstractValidatorBase<QuestionnaireRequest>
{
    public QuestionnaireRequestValidator()
    {
        _ = RuleFor(x => x.SapVendorNumber).NotEmpty();
        _ = RuleFor(x => x.IsSubmitQuestionnaire).NotNull();
        _ = RuleFor(x => x.Answers).Must((request, answers) =>
        {
            var mandatoryNumbers = EBVL.Shared.Statics.VendorRegistrations.QuestionnaireFor.All
                .Where(question => question.IsRequired && (!request.IsNotSoleAgent || question.Section != EBVL.Shared.Statics.VendorRegistrations.QuestionnaireFor.SoleAgent))
                .Select(question => question.Number)
                .ToHashSet();

            return answers
                .Where(answer => mandatoryNumbers.Contains(answer.QuestionNumber))
                .All(answer => !string.IsNullOrWhiteSpace(answer.Value) || !string.IsNullOrWhiteSpace(answer.FileName));
        }).WithMessage("Seluruh pertanyaan wajib harus diisi.");
    }
}
