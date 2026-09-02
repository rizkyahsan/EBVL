using EBVL.Shared.Dto.Modules.Main.VendorRegistrations.DocumentEvidence;

namespace EBVL.FrontEnd.WebUi.Modules.Main.Features.VendorRegistrations.Components;

public partial class DocumentEvidenceForm
{
    private const long MaximumFileSize = 25 * 1024 * 1024;

    [Parameter]
    public required DocumentEvidenceRequest Model { get; init; }

    [Parameter]
    public required Func<string, IBrowserFile, Task> OnFileSelected { get; init; }

    [Parameter]
    public required Func<string, Task> OnFileRemoved { get; init; }

    [Parameter]
    public required EventCallback<DocumentEvidenceRequest> OnSubmit { get; init; }

    private readonly DocumentEvidenceRequestValidator _validator = new();
    private string? _validationError;
    private DocumentEvidenceItemRequest GetDocument(string key)
    {
        return Model.Documents.Single(document => document.Key == key);
    }

    private static string GetDisplayName(string key, string defaultName)
    {
        return key switch
        {
            "ProductExperienceList" => "Project Portfolio",
            "CompanyTaxCard" => "Company Tax Card",
            "PrimaryCertificate" => "Certificate",
            _ => defaultName
        };
    }

    private async Task SelectFile(string key, InputFileChangeEventArgs eventArgs)
    {
        var file = eventArgs.File;
        if (!string.Equals(Path.GetExtension(file.Name), ".pdf", StringComparison.OrdinalIgnoreCase) ||
            !string.Equals(file.ContentType, "application/pdf", StringComparison.OrdinalIgnoreCase))
        {
            _validationError = "Only PDF files are allowed.";
            return;
        }

        if (file.Size > MaximumFileSize)
        {
            _validationError = "The maximum file size is 25 MB.";
            return;
        }

        _validationError = null;
        await OnFileSelected(key, file);
    }

    private Task RemoveFile(string key)
    {
        return OnFileRemoved(key);
    }

    public async Task SubmitAsync()
    {
        var result = await _validator.ValidateAsync(Model);

        if (!result.IsValid)
        {
            _validationError = result.Errors.First().ErrorMessage;
            return;
        }

        _validationError = null;
        await OnSubmit.InvokeAsync(Model);
    }
}
