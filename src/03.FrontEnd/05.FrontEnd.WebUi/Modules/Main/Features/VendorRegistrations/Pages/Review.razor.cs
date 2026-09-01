using EBVL.FrontEnd.WebUi.Modules.Main.Features.VendorRegistrations.Components;
using EBVL.FrontEnd.WebUi.Modules.Main.Features.VendorRegistrations.Services;
using EBVL.Shared.Dto.Modules.Main.VendorRegistrations.DocumentEvidence;
using EBVL.Shared.Dto.Modules.Main.VendorRegistrations.PreRegistration;
using Microsoft.JSInterop;
using VendorRegistrationRouteFor = EBVL.FrontEnd.WebUi.Modules.Main.Features.VendorRegistrations.Statics.RouteFor;

namespace EBVL.FrontEnd.WebUi.Modules.Main.Features.VendorRegistrations.Pages;

public partial class Review
{
    private const long MaximumReviewFileSize = 25 * 1024 * 1024;

    [Inject]
    public required NavigationManager NavigationManager { get; init; }

    [Inject]
    public required VendorRegistrationState RegistrationState { get; init; }

    [Inject]
    public required IDialogService DialogService { get; init; }

    [Inject]
    public required ISnackbar Snackbar { get; init; }

    [Inject]
    public required IJSRuntime JSRuntime { get; init; }

    private PreRegistrationRequest _preRegistration = new();
    private DocumentEvidenceRequest _documentEvidence = new();
    private bool _isRestoring = true;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender)
        {
            return;
        }

        await RegistrationState.RestoreAsync();

        if (!RegistrationState.IsStepTwoCompleted)
        {
            NavigationManager.NavigateTo(VendorRegistrationRouteFor.StepTwo);
            return;
        }

        if (!RegistrationState.IsStepThreeCompleted ||
            RegistrationState.PreRegistration is null ||
            RegistrationState.DocumentEvidence is null)
        {
            NavigationManager.NavigateTo(VendorRegistrationRouteFor.StepThree);
            return;
        }

        _preRegistration = RegistrationState.PreRegistration;
        _documentEvidence = RegistrationState.DocumentEvidence;
        _isRestoring = false;
        await InvokeAsync(StateHasChanged);
    }

    private async Task DownloadDocument(string key)
    {
        if (!RegistrationState.SelectedFiles.TryGetValue(key, out var file))
        {
            Snackbar.AddError("File tidak tersedia. Silakan upload ulang dokumen.");
            return;
        }

        await using var stream = file.OpenReadStream(MaximumReviewFileSize);
        using var memoryStream = new MemoryStream();
        await stream.CopyToAsync(memoryStream);
        var base64Data = Convert.ToBase64String(memoryStream.ToArray());
        await JSRuntime.InvokeVoidAsync(JavaScriptIdentifierFor.DownloadFile, file.Name, file.ContentType, base64Data);
    }

    private async Task SendForVerification()
    {
        var dialog = await DialogService.ShowAsync<DialogSendVendorData>(string.Empty, new DialogOptions
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

        Snackbar.AddSuccess("Data vendor berhasil dikirim untuk verifikasi.");
    }
}
