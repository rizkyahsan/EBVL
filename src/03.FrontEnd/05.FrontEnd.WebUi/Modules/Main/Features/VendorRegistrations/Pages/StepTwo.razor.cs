using EBVL.FrontEnd.Logics.Modules.Main.VendorRegistrations.Questionnaires;
using EBVL.FrontEnd.WebUi.Modules.Main.Features.VendorRegistrations.Components;
using EBVL.FrontEnd.WebUi.Modules.Main.Features.VendorRegistrations.Services;
using EBVL.Shared.Dto.Modules.Main.VendorRegistrations.Questionnaires;
using MediatR;
using VendorRegistrationRouteFor = EBVL.FrontEnd.WebUi.Modules.Main.Features.VendorRegistrations.Statics.RouteFor;

namespace EBVL.FrontEnd.WebUi.Modules.Main.Features.VendorRegistrations.Pages;

public partial class StepTwo
{
    #region Dependencies

    [Inject]
    public required NavigationManager NavigationManager { get; init; }

    [Inject]
    public required VendorRegistrationState RegistrationState { get; init; }
    [Inject] public required ISender Sender { get; init; }
    [Inject] public required ISnackbar Snackbar { get; init; }

    #endregion

    #region Fields

    private IReadOnlyList<VendorRegistrationDocumentItem> _documents = [];
    private bool _isRestoring = true;
    private DocumentEvidenceForm? _documentEvidenceForm;

    #endregion

    #region Lifecycle

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender)
        {
            return;
        }

        if (!await RegistrationState.RestoreRuntimeAsync(Sender) || RegistrationState.Runtime is null)
        {
            NavigationManager.NavigateTo(VendorRegistrationRouteFor.StepOne);
            return;
        }

        _documents = RegistrationState.Runtime.Documents;

        _isRestoring = false;
        await InvokeAsync(StateHasChanged);
    }

    #endregion

    #region Private Methods

    private void BackToGuidance()
    {
        NavigationManager.NavigateTo(VendorRegistrationRouteFor.Index);
    }

    private void BackToStepOne()
    {
        var sapVendorNumber = RegistrationState.PreRegistration?.SapVendorNumber;
        NavigationManager.NavigateTo(string.IsNullOrWhiteSpace(sapVendorNumber)
            ? VendorRegistrationRouteFor.StepOne
            : VendorRegistrationRouteFor.StepOneWith(sapVendorNumber));
    }

    private async Task SelectFile(VendorRegistrationDocumentItem definition, IBrowserFile file)
    {
        try
        {
            await using var stream = file.OpenReadStream(definition.MaxSizeMb * 1024L * 1024L);
            using var memory = new MemoryStream();
            await stream.CopyToAsync(memory);
            var response = await Sender.Send(new UploadVendorRegistrationDocumentCommand(RegistrationState.RegistrationId!.Value, definition.DefinitionKey, RegistrationState.ResumeToken!, file.Name, memory.ToArray()));
            _documents = _documents.Select(item => item.RequirementId == response.Document.RequirementId ? response.Document : item).ToList();
            await RefreshRuntime();
        }
        catch (Exception exception)
        {
            Snackbar.AddError(exception.Message);
            throw;
        }
    }

    private async Task RemoveFile(VendorRegistrationDocumentItem definition)
    {
        if (definition.DocumentId is null)
        {
            return;
        }

        await Sender.Send(new DeleteVendorRegistrationDocumentCommand(RegistrationState.RegistrationId!.Value, definition.DocumentId.Value, RegistrationState.ResumeToken!));
        await RefreshRuntime();
    }

    private Task ContinueRegistration()
    {
        NavigationManager.NavigateTo(VendorRegistrationRouteFor.StepThree);
        return Task.CompletedTask;
    }

    private async Task RefreshRuntime()
    {
        var runtime = await Sender.Send(new GetQuestionnaireRuntimeQuery(RegistrationState.RegistrationId!.Value, RegistrationState.ResumeToken!));
        await RegistrationState.SetRuntimeAsync(runtime);
        _documents = runtime.Documents;
    }

    private Task SubmitDocumentEvidence()
    {
        return _documentEvidenceForm?.SubmitAsync() ?? Task.CompletedTask;
    }

    #endregion
}
