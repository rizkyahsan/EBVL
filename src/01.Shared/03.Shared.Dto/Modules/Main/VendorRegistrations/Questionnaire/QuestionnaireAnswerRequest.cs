namespace EBVL.Shared.Dto.Modules.Main.VendorRegistrations.Questionnaire;

public sealed record QuestionnaireAnswerRequest
{
    public required int QuestionNumber { get; init; }
    public string Value { get; set; } = string.Empty;
    public string? FileName { get; set; }
}
