using EBVL.Shared.Dto.Modules.Main.VendorRegistrations.Questionnaire;
using EBVL.Shared.Statics.VendorRegistrations;

namespace EBVL.FrontEnd.WebUi.Modules.Main.Features.VendorRegistrations.Components;

public partial class QuestionnaireSection
{
    [Parameter]
    public required string Title { get; init; }

    [Parameter]
    public required IReadOnlyList<VendorQuestionDefinition> Questions { get; init; }

    [Parameter]
    public required QuestionnaireRequest Model { get; init; }

    [Parameter]
    public RenderFragment? HeaderContent { get; init; }

    [Parameter]
    public bool Disabled { get; init; }

    [Parameter]
    public EventCallback OnChanged { get; init; }

    private QuestionnaireAnswerRequest GetAnswer(int questionNumber)
    {
        return Model.Answers.Single(answer => answer.QuestionNumber == questionNumber);
    }

    private async Task UpdateAnswer(int questionNumber, string? value)
    {
        GetAnswer(questionNumber).Value = value ?? string.Empty;
        await OnChanged.InvokeAsync();
    }

    private async Task UploadFile(int questionNumber, InputFileChangeEventArgs eventArgs)
    {
        GetAnswer(questionNumber).FileName = eventArgs.File.Name;
        await OnChanged.InvokeAsync();
    }
}
