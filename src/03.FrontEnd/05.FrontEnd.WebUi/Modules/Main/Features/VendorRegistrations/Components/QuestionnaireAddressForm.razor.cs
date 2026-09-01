using EBVL.Shared.Dto.Modules.Main.VendorRegistrations.Questionnaire;

namespace EBVL.FrontEnd.WebUi.Modules.Main.Features.VendorRegistrations.Components;

public partial class QuestionnaireAddressForm
{
    [Parameter]
    public required string Title { get; init; }

    [Parameter]
    public required QuestionnaireAddressRequest Model { get; init; }

    [Parameter]
    public bool Required { get; init; }

    [Parameter]
    public EventCallback OnChanged { get; init; }

    private async Task Update(string? value, string propertyName)
    {
        var property = typeof(QuestionnaireAddressRequest).GetProperty(propertyName)!;
        property.SetValue(Model, value ?? string.Empty);
        await OnChanged.InvokeAsync();
    }
}
