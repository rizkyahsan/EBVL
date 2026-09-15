using System.Text.Json;
using EBVL.Shared.Dto.Modules.Main.VendorRegistrations.DocumentEvidence;
using EBVL.Shared.Dto.Modules.Main.VendorRegistrations.PreRegistration;
using EBVL.Shared.Dto.Modules.Main.VendorRegistrations.Questionnaire;
using EBVL.Shared.Dto.Modules.Main.VendorRegistrations.Questionnaires;
using MediatR;
using Microsoft.JSInterop;

namespace EBVL.FrontEnd.WebUi.Modules.Main.Features.VendorRegistrations.Services;

public sealed class VendorRegistrationState(IJSRuntime jsRuntime)
{
    #region Constants

    private const string PreRegistrationKey = "ebvl.vendor-registration.step-one";
    private const string QuestionnaireKey = "ebvl.vendor-registration.step-two";
    private const string DocumentEvidenceKey = "ebvl.vendor-registration.step-three";
    private const string VerificationSentKey = "ebvl.vendor-registration.verification-sent";
    private const string EmailVerifiedKey = "ebvl.vendor-registration.email-verified";
    private const string RuntimeKey = "ebvl.vendor-registration.runtime";

    #endregion

    #region Properties

    public PreRegistrationRequest? PreRegistration { get; private set; }
    public QuestionnaireRequest? Questionnaire { get; private set; }
    public DocumentEvidenceRequest? DocumentEvidence { get; private set; }
    public IReadOnlyDictionary<string, IBrowserFile> SelectedFiles => _selectedFiles;
    public bool IsStepOneCompleted => PreRegistration is not null;
    public bool IsStepTwoCompleted => DocumentEvidence is not null &&
        DocumentEvidence.Documents.All(document => !string.IsNullOrWhiteSpace(document.FileName));
    public bool IsStepThreeCompleted => Questionnaire?.IsSubmitQuestionnaire is true;
    public bool IsVerificationSent { get; private set; }
    public bool IsEmailVerified { get; private set; }
    public Guid? RegistrationId { get; private set; }
    public string? ResumeToken { get; private set; }
    public QuestionnaireRuntimeResponse? Runtime { get; private set; }

    #endregion

    #region Fields

    private readonly Dictionary<string, IBrowserFile> _selectedFiles = [];

    #endregion

    #region Public Methods

    public async Task CompleteStepOneAsync(PreRegistrationRequest request)
    {
        PreRegistration = request;
        DocumentEvidence ??= new DocumentEvidenceRequest
        {
            SapVendorNumber = request.SapVendorNumber
        };

        await PersistAsync();
    }

    public async Task CompleteStepTwoAsync(DocumentEvidenceRequest request)
    {
        DocumentEvidence = request;
        Questionnaire ??= new QuestionnaireRequest
        {
            SapVendorNumber = request.SapVendorNumber
        };
        await PersistAsync();
    }

    public async Task CompleteStepThreeAsync(QuestionnaireRequest request)
    {
        Questionnaire = request;
        await PersistAsync();
    }

    public async Task UpdateStepThreeAsync(QuestionnaireRequest request)
    {
        Questionnaire = request;
        await PersistAsync();
    }

    public async Task RestoreAsync()
    {
        var preRegistrationJson = await jsRuntime.InvokeAsync<string?>("sessionStorage.getItem", PreRegistrationKey);
        var questionnaireJson = await jsRuntime.InvokeAsync<string?>("sessionStorage.getItem", QuestionnaireKey);
        var documentEvidenceJson = await jsRuntime.InvokeAsync<string?>("sessionStorage.getItem", DocumentEvidenceKey);
        var verificationSent = await jsRuntime.InvokeAsync<string?>("sessionStorage.getItem", VerificationSentKey);
        var emailVerified = await jsRuntime.InvokeAsync<string?>("sessionStorage.getItem", EmailVerifiedKey);
        var runtimeJson = await jsRuntime.InvokeAsync<string?>("sessionStorage.getItem", RuntimeKey);

        IsVerificationSent = bool.TryParse(verificationSent, out var isSent) && isSent;
        IsEmailVerified = bool.TryParse(emailVerified, out var isVerified) && isVerified;
        if (!string.IsNullOrWhiteSpace(runtimeJson))
        {
            var identity = JsonSerializer.Deserialize<RuntimeIdentity>(runtimeJson);
            RegistrationId = identity?.RegistrationId;
            ResumeToken = identity?.ResumeToken;
        }

        if (!string.IsNullOrWhiteSpace(preRegistrationJson))
        {
            PreRegistration = JsonSerializer.Deserialize<PreRegistrationRequest>(preRegistrationJson);
        }

        if (!string.IsNullOrWhiteSpace(questionnaireJson))
        {
            Questionnaire = JsonSerializer.Deserialize<QuestionnaireRequest>(questionnaireJson);
        }

        if (!string.IsNullOrWhiteSpace(documentEvidenceJson))
        {
            DocumentEvidence = JsonSerializer.Deserialize<DocumentEvidenceRequest>(documentEvidenceJson);
        }
    }

    public async Task SetRuntimeAsync(QuestionnaireRuntimeResponse runtime, string? resumeToken = null)
    {
        Runtime = runtime;
        RegistrationId = runtime.RegistrationId;
        ResumeToken = resumeToken ?? runtime.ResumeToken ?? ResumeToken;
        PreRegistration = runtime.Profile;
        await jsRuntime.InvokeVoidAsync("sessionStorage.setItem", RuntimeKey, JsonSerializer.Serialize(new RuntimeIdentity(runtime.RegistrationId, ResumeToken!)));
        await PersistAsync();
    }

    public async Task<bool> RestoreRuntimeAsync(ISender sender)
    {
        await RestoreAsync();
        if (RegistrationId is null || string.IsNullOrWhiteSpace(ResumeToken))
        {
            return false;
        }

        Runtime = await sender.Send(new Logics.Modules.Main.VendorRegistrations.Questionnaires.GetQuestionnaireRuntimeQuery(RegistrationId.Value, ResumeToken));
        PreRegistration = Runtime.Profile;
        return true;
    }

    public async Task MarkVerificationSentAsync()
    {
        IsVerificationSent = true;
        await jsRuntime.InvokeVoidAsync("sessionStorage.setItem", VerificationSentKey, bool.TrueString);
    }

    public async Task MarkEmailVerifiedAsync()
    {
        IsEmailVerified = true;
        await jsRuntime.InvokeVoidAsync("sessionStorage.setItem", EmailVerifiedKey, bool.TrueString);
    }

    public async Task SetDocumentAsync(string key, IBrowserFile file)
    {
        DocumentEvidence ??= new DocumentEvidenceRequest
        {
            SapVendorNumber = PreRegistration?.SapVendorNumber ?? string.Empty
        };

        var document = DocumentEvidence.Documents.Single(item => item.Key == key);
        document.FileName = file.Name;
        _selectedFiles[key] = file;
        await PersistAsync();
    }

    public async Task RemoveDocumentAsync(string key)
    {
        if (DocumentEvidence is null)
        {
            return;
        }

        DocumentEvidence.Documents.Single(item => item.Key == key).FileName = null;
        _ = _selectedFiles.Remove(key);
        await PersistAsync();
    }

    #endregion

    #region Private Methods

    private async Task PersistAsync()
    {
        if (PreRegistration is not null)
        {
            await jsRuntime.InvokeVoidAsync("sessionStorage.setItem", PreRegistrationKey, JsonSerializer.Serialize(PreRegistration));
        }

        if (Questionnaire is not null)
        {
            await jsRuntime.InvokeVoidAsync("sessionStorage.setItem", QuestionnaireKey, JsonSerializer.Serialize(Questionnaire));
        }

        if (DocumentEvidence is not null)
        {
            await jsRuntime.InvokeVoidAsync("sessionStorage.setItem", DocumentEvidenceKey, JsonSerializer.Serialize(DocumentEvidence));
        }
    }

    #endregion

    #region Nested Types

    private sealed record RuntimeIdentity(Guid RegistrationId, string ResumeToken);

    #endregion
}
