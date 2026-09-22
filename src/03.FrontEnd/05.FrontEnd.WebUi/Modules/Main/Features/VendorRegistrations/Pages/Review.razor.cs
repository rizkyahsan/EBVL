using EBVL.FrontEnd.Logics.Modules.Main.VendorRegistrations.Questionnaires;
using EBVL.FrontEnd.WebUi.Modules.Main.Features.VendorRegistrations.Components;
using EBVL.FrontEnd.WebUi.Modules.Main.Features.VendorRegistrations.Services;
using EBVL.Shared.Dto.Modules.Main.VendorRegistrations.PreRegistration;
using EBVL.Shared.Dto.Modules.Main.VendorRegistrations.Questionnaires;
using MediatR;
using Microsoft.JSInterop;
using VendorRegistrationRouteFor = EBVL.FrontEnd.WebUi.Modules.Main.Features.VendorRegistrations.Statics.RouteFor;

namespace EBVL.FrontEnd.WebUi.Modules.Main.Features.VendorRegistrations.Pages;

public partial class Review
{
    #region Dependencies

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
    [Inject] public required ISender Sender { get; init; }

    #endregion

    #region Fields

    private PreRegistrationRequest _preRegistration = new();
    private QuestionnaireRuntimeResponse _runtime = default!;
    private bool _isRestoring = true;
    private bool _isSubmitting;

    #endregion

    #region Lifecycle

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender)
        {
            return;
        }

        if (!await RegistrationState.RestoreRuntimeAsync(Sender) || RegistrationState.Runtime is null || !RegistrationState.Runtime.IsDocumentEvidenceComplete)
        {
            NavigationManager.NavigateTo(VendorRegistrationRouteFor.StepTwo);
            return;
        }

        if (RegistrationState.PreRegistration is null)
        {
            NavigationManager.NavigateTo(VendorRegistrationRouteFor.StepThree);
            return;
        }

        _preRegistration = RegistrationState.PreRegistration;
        _runtime = RegistrationState.Runtime;
        _isRestoring = false;
        await InvokeAsync(StateHasChanged);
    }

    #endregion

    #region Private Methods

    private async Task DownloadDocument(VendorRegistrationDocumentItem document)
    {
        if (document.DocumentId is null)
        {
            return;
        }

        var file = await Sender.Send(new DownloadVendorRegistrationDocumentQuery(_runtime.RegistrationId, document.DocumentId.Value, RegistrationState.ResumeToken!));
        await Download(file);
    }

    private async Task DownloadFile(RuntimeFileItem item)
    {
        await Download(await Sender.Send(new DownloadQuestionnaireFileQuery(_runtime.RegistrationId, item.Id, RegistrationState.ResumeToken!)));
    }

    private Task Download(QuestionnaireFileContent file)
    {
        return JSRuntime.InvokeVoidAsync(JavaScriptIdentifierFor.DownloadFile, file.FileName, file.ContentType, Convert.ToBase64String(file.Content)).AsTask();
    }

    private async Task SendForVerification()
    {
        if (_isSubmitting)
        {
            return;
        }

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

        try
        {
            _isSubmitting = true;
            var runtime = await Sender.Send(new SubmitQuestionnaireCommand(_runtime.RegistrationId, new(RegistrationState.ResumeToken!, _runtime.RowVersion)));
            await RegistrationState.SetRuntimeAsync(runtime);
            await RegistrationState.MarkVerificationSentAsync();
            NavigationManager.NavigateTo(VendorRegistrationRouteFor.EmailVerification);
        }
        catch (Exception exception)
        {
            Snackbar.AddError(exception.Message);
        }
        finally
        {
            _isSubmitting = false;
        }
    }

    private void BackToGuidance()
    {
        NavigationManager.NavigateTo(VendorRegistrationRouteFor.Index);
    }

    private void BackToStepThree()
    {
        NavigationManager.NavigateTo(VendorRegistrationRouteFor.StepThree);
    }

    #endregion
}
