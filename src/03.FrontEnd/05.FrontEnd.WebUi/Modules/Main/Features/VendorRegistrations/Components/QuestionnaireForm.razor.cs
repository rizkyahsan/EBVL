using EBVL.Shared.Dto.Modules.Main.VendorRegistrations.Questionnaire;

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

    private Task NotifyChanged()
    {
        return OnChanged.InvokeAsync(Model);
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
