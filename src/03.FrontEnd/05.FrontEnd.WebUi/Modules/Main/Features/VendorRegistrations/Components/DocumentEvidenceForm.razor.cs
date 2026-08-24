using EBVL.Shared.Dto.Modules.Main.VendorRegistrations.DocumentEvidence;

namespace EBVL.FrontEnd.WebUi.Modules.Main.Features.VendorRegistrations.Components;

public partial class DocumentEvidenceForm
{
    [Parameter]
    public required DocumentEvidenceRequest Model { get; init; }

    [Parameter]
    public required IEnumerable<string> AvailableFileKeys { get; init; }

    [Parameter]
    public required Func<string, IBrowserFile, Task> OnFileSelected { get; init; }

    [Parameter]
    public required Func<string, Task> OnFileRemoved { get; init; }

    [Parameter]
    public required EventCallback OnBack { get; init; }

    [Parameter]
    public required EventCallback<DocumentEvidenceRequest> OnSubmit { get; init; }

    private readonly DocumentEvidenceRequestValidator _validator = new();
    private string? _validationError;
    private bool IsComplete => Model.Documents.All(document =>
        !string.IsNullOrWhiteSpace(document.FileName) && AvailableFileKeys.Contains(document.Key));

    private DocumentEvidenceItemRequest GetDocument(string key)
    {
        return Model.Documents.Single(document => document.Key == key);
    }

    private static int GetDocumentNumber(string key)
    {
        return EBVL.Shared.Statics.VendorRegistrations.DocumentEvidenceFor.All
            .Select((document, index) => new { document.Key, Number = index + 1 })
            .Single(document => document.Key == key).Number;
    }

    private static string GetDocumentTitleClass(bool isPrimaryCertificate)
    {
        return isPrimaryCertificate ? "document-title primary-document-title" : "document-title";
    }

    private static string GetIcon(string key)
    {
        return key switch
        {
            "BrandRegistrationLetter" => Icons.Material.Filled.ContactPage,
            "CompanyProfile" => Icons.Material.Filled.Business,
            "ProductCatalog" => Icons.Material.Filled.MenuBook,
            "ProductExperienceList" => Icons.Material.Filled.History,
            "CompanyTaxCard" => Icons.Material.Filled.CreditCard,
            _ => Icons.Material.Filled.VerifiedUser
        };
    }

    private async Task SelectFile(string key, InputFileChangeEventArgs eventArgs)
    {
        await OnFileSelected(key, eventArgs.File);
    }

    private Task RemoveFile(string key)
    {
        return OnFileRemoved(key);
    }

    private async Task Submit()
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
