using EBVL.Shared.Dto.Modules.Main.VendorRegistrations.PreRegistration;
using EBVL.Shared.Dto.Modules.Main.VendorRegistrations.Questionnaire;
using EBVL.Shared.Dto.Modules.Main.VendorRegistrations.DocumentEvidence;
using System.Text.Json;
using Microsoft.JSInterop;

namespace EBVL.FrontEnd.WebUi.Modules.Main.Features.VendorRegistrations.Services;

public sealed class VendorRegistrationState(IJSRuntime jsRuntime)
{
    private const string PreRegistrationKey = "ebvl.vendor-registration.step-one";
    private const string QuestionnaireKey = "ebvl.vendor-registration.step-two";
    private const string DocumentEvidenceKey = "ebvl.vendor-registration.step-three";

    public PreRegistrationRequest? PreRegistration { get; private set; }
    public QuestionnaireRequest? Questionnaire { get; private set; }
    public DocumentEvidenceRequest? DocumentEvidence { get; private set; }
    public IReadOnlyDictionary<string, IBrowserFile> SelectedFiles => _selectedFiles;
    public bool IsStepOneCompleted => PreRegistration is not null;
    public bool IsStepTwoCompleted => DocumentEvidence is not null &&
        DocumentEvidence.Documents.All(document => !string.IsNullOrWhiteSpace(document.FileName));
    public bool IsStepThreeCompleted => Questionnaire?.IsSubmitQuestionnaire is true;
    private readonly Dictionary<string, IBrowserFile> _selectedFiles = [];

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
}
