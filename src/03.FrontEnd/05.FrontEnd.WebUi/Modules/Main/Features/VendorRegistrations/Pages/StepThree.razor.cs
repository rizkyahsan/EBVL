using EBVL.FrontEnd.WebUi.Modules.Main.Features.VendorRegistrations.Components;
using EBVL.FrontEnd.WebUi.Modules.Main.Features.VendorRegistrations.Services;
using EBVL.Shared.Dto.Modules.Main.VendorRegistrations.DocumentEvidence;
using VendorRegistrationRouteFor = EBVL.FrontEnd.WebUi.Modules.Main.Features.VendorRegistrations.Statics.RouteFor;

namespace EBVL.FrontEnd.WebUi.Modules.Main.Features.VendorRegistrations.Pages;

public partial class StepThree
{
    [Inject]
    public required NavigationManager NavigationManager { get; init; }

    [Inject]
    public required VendorRegistrationState RegistrationState { get; init; }

    [Inject]
    public required IDialogService DialogService { get; init; }

    private DocumentEvidenceRequest _model = new();
    private bool _isRestoring = true;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender)
        {
            return;
        }

        await RegistrationState.RestoreAsync();

        if (RegistrationState.Questionnaire?.IsSubmitQuestionnaire is not true || RegistrationState.PreRegistration is null)
        {
            NavigationManager.NavigateTo(VendorRegistrationRouteFor.StepTwo);
            return;
        }

        _model = RegistrationState.DocumentEvidence ?? new DocumentEvidenceRequest
        {
            SapVendorNumber = RegistrationState.PreRegistration.SapVendorNumber
        };

        _isRestoring = false;
        await InvokeAsync(StateHasChanged);
    }

    private async Task SelectFile(string key, IBrowserFile file)
    {
        await RegistrationState.SetDocumentAsync(key, file);
        _model = RegistrationState.DocumentEvidence!;
    }

    private async Task RemoveFile(string key)
    {
        await RegistrationState.RemoveDocumentAsync(key);
        _model = RegistrationState.DocumentEvidence!;
    }

    private void BackToStepTwo()
    {
        NavigationManager.NavigateTo(VendorRegistrationRouteFor.StepTwo);
    }

    private async Task SubmitRegistration(DocumentEvidenceRequest model)
    {
        var dialog = await DialogService.ShowAsync<DialogSubmitDocumentEvidence>(string.Empty, new DialogOptions
        {
            MaxWidth = MaxWidth.Small,
            FullWidth = true,
            CloseButton = true
        });
        var result = await dialog.Result;

        if (result is null || result.Canceled)
        {
            return;
        }

        NavigationManager.NavigateTo(VendorRegistrationRouteFor.Review);
    }
}
