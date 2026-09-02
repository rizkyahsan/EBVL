using EBVL.Shared.Dto.Modules.Main.VendorRegistrations.Questionnaire;
using System.Text.Json;

namespace EBVL.FrontEnd.WebUi.Modules.Main.Features.VendorRegistrations.Components;

public partial class QuestionnaireGeneralForm
{
    [Parameter]
    public required QuestionnaireRequest Model { get; init; }

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

    private async Task AddressChanged(int questionNumber, QuestionnaireAddressRequest address)
    {
        GetAnswer(questionNumber).Value = JsonSerializer.Serialize(address);
        await OnChanged.InvokeAsync();
    }
}
