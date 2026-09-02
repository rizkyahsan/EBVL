using EBVL.Shared.Dto.Modules.Main.VendorRegistrations.Questionnaire;

namespace EBVL.FrontEnd.WebUi.Modules.Main.Features.VendorRegistrations.Components;

public partial class QuestionnaireUploadField
{
    [Parameter]
    public required string Label { get; init; }

    [Parameter]
    public string? Mark { get; init; }

    [Parameter]
    public required QuestionnaireAnswerRequest Answer { get; init; }

    [Parameter]
    public EventCallback OnChanged { get; init; }

    private string InputId => $"questionnaire-file-{Answer.QuestionNumber}";

    private async Task UploadFile(InputFileChangeEventArgs eventArgs)
    {
        Answer.FileName = eventArgs.File.Name;
        await OnChanged.InvokeAsync();
    }

    private async Task RemoveFile()
    {
        Answer.FileName = null;
        await OnChanged.InvokeAsync();
    }
}
