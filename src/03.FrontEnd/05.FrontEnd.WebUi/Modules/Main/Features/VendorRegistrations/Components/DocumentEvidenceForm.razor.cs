using EBVL.Shared.Dto.Modules.Main.VendorRegistrations.Questionnaires;

namespace EBVL.FrontEnd.WebUi.Modules.Main.Features.VendorRegistrations.Components;

public partial class DocumentEvidenceForm
{
    #region Parameters

    [Parameter]
    public required IReadOnlyList<VendorRegistrationDocumentItem> Documents { get; init; }

    [Parameter]
    public required Func<VendorRegistrationDocumentItem, IBrowserFile, Task> OnFileSelected { get; init; }

    [Parameter]
    public required Func<VendorRegistrationDocumentItem, Task> OnFileRemoved { get; init; }

    [Parameter]
    public required EventCallback OnSubmit { get; init; }

    #endregion

    #region Fields

    private readonly Dictionary<string, string?> _fileNames = [];
    private string? _validationError;

    #endregion

    #region Lifecycle

    protected override void OnParametersSet()
    {
        foreach (var document in Documents)
        {
            if (!_fileNames.ContainsKey(document.DefinitionKey))
            {
                _fileNames[document.DefinitionKey] = document.FileName;
            }
        }
    }

    #endregion

    #region Public Methods

    public async Task SubmitAsync()
    {
        var missing = Documents.FirstOrDefault(item => item.IsMandatory
            && item.DocumentId is null
            && string.IsNullOrWhiteSpace(_fileNames.GetValueOrDefault(item.DefinitionKey) ?? item.FileName));
        if (missing is not null)
        {
            _validationError = $"{missing.Name} is required.";
            return;
        }

        _validationError = null;
        await OnSubmit.InvokeAsync();
    }

    #endregion

    #region Private Methods

    private async Task SelectFile(VendorRegistrationDocumentItem definition, InputFileChangeEventArgs eventArgs)
    {
        var file = eventArgs.File;
        if (!string.Equals(Path.GetExtension(file.Name), ".pdf", StringComparison.OrdinalIgnoreCase) ||
            !string.Equals(file.ContentType, "application/pdf", StringComparison.OrdinalIgnoreCase))
        {
            _validationError = "Only PDF files are allowed.";
            return;
        }

        if (file.Size > definition.MaxSizeMb * 1024L * 1024L)
        {
            _validationError = $"The maximum file size is {definition.MaxSizeMb} MB.";
            return;
        }

        _validationError = null;
        _fileNames[definition.DefinitionKey] = file.Name;
        await InvokeAsync(StateHasChanged);
        await OnFileSelected(definition, file);
    }

    private Task RemoveFile(VendorRegistrationDocumentItem definition)
    {
        _ = _fileNames.Remove(definition.DefinitionKey);
        return OnFileRemoved(definition);
    }

    #endregion
}
