using EBVL.FrontEnd.WebUi.Common.Components.Abstracts;
using EBVL.FrontEnd.WebUi.Modules.Main.Features.Brand.Registers.Components;
using EBVL.FrontEnd.WebUi.Modules.Main.Features.Brand.Registers.Services;
using EBVL.Shared.Dto.Modules.Main.BrandRegistrations.CreateBrandRegistration;

namespace EBVL.FrontEnd.WebUi.Modules.Main.Features.Brand.Registers.Pages;

public partial class Create : PageBase
{
    [Inject]
    public required BrandRegistrationState RegistrationState { get; init; }

    protected MudForm Form { get; set; } = default!;
    protected BrandFormModel Model { get; set; } = new();
    protected IReadOnlyList<string> Brands { get; } = ["Kitz", "PowerShell", "Textile Horizon", "NEWCO", "De Wallt", "Samsung", "CAT", "TEKO", "VALVE", "Claude"];
    protected IReadOnlyList<string> Groups { get; } = ["Industrial Equipment", "Power Tools", "Uniform", "Electronics", "Sanitary", "Technology"];
    protected IReadOnlyList<string> Categories { get; } = ["Goods", "Services", "Technology", "Safety Equipment"];
    protected IReadOnlyList<string> Countries { get; } = ["Indonesia", "Japan", "United States", "Germany", "South Korea", "Singapore"];
    protected IReadOnlyList<DocumentUploadItem> AdministrativeDocuments { get; private set; } = CreateDocuments("administrative");
    protected IReadOnlyList<DocumentUploadItem> TechnicalDocuments { get; private set; } = CreateDocuments("technical");
    protected bool _showValidation;
    protected bool _isSubmitting;
    protected int _fileInputVersion;

    protected override void OnInitialized()
    {
        LoadBreadcrumbs();
    }

    protected override void LoadBreadcrumbs()
    {
        _breadcrumbItems =
        [
            MainBreadcrumbFor.Home,
            BrandRegistersBreadcrumbFor.Brand,
            new BreadcrumbItem(BrandRegistersDisplayTextFor.Register, BrandRegistersRouteFor.Index),
            CommonBreadcrumbFor.Active(BrandRegistersDisplayTextFor.CreateNewBrand)
        ];
    }

    protected void SelectFile(DocumentUploadItem document, InputFileChangeEventArgs args)
    {
        document.FileName = args.File.Name;
    }

    protected void RemoveFile(DocumentUploadItem document)
    {
        document.FileName = null;
        _fileInputVersion++;
    }

    protected async Task Reset()
    {
        var dialog = await DialogService.ShowAsync<DialogConfirmReset>(string.Empty, ConfirmationDialogOptions);
        var result = await dialog.Result;
        if (result is null || result.Canceled)
        {
            return;
        }

        Model = new BrandFormModel();
        AdministrativeDocuments = CreateDocuments("administrative");
        TechnicalDocuments = CreateDocuments("technical");
        _showValidation = false;
        _fileInputVersion++;
        await Form.ResetAsync();
    }

    protected async Task SaveDraft()
    {
        var dialog = await DialogService.ShowAsync<DialogConfirmSaveDraft>(string.Empty, ConfirmationDialogOptions);
        var result = await dialog.Result;
        if (result is null || result.Canceled)
        {
            return;
        }

        try
        {
            _ = await RegistrationState.AddAsync(CreateRequest(), false);
            _ = Snackbar.Add("Brand berhasil disimpan sebagai Draft.", MudBlazor.Severity.Success);
            NavigationManager.NavigateTo(BrandRegistersRouteFor.Index);
        }
        catch (Exception exception)
        {
            _ = Snackbar.Add(exception.Message, MudBlazor.Severity.Error);
        }
    }

    protected async Task Submit()
    {
        if (_isSubmitting)
        {
            return;
        }

        try
        {
            _showValidation = true;
            await Form.Validate();
            var hasMissingDocuments = AdministrativeDocuments.Concat(TechnicalDocuments)
                .Any(document => document.IsMandatory && string.IsNullOrWhiteSpace(document.FileName));
            if (!Form.IsValid || hasMissingDocuments)
            {
                _ = Snackbar.Add("Lengkapi seluruh field dan dokumen wajib sebelum submit.", MudBlazor.Severity.Error);
                return;
            }

            var dialog = await DialogService.ShowAsync<DialogConfirmSubmit>(string.Empty, ConfirmationDialogOptions);
            var result = await dialog.Result;
            if (result is null || result.Canceled)
            {
                return;
            }

            _isSubmitting = true;
            await InvokeAsync(StateHasChanged);
            _ = await RegistrationState.AddAsync(CreateRequest(), true);
            _ = Snackbar.Add("Brand berhasil disubmit.", MudBlazor.Severity.Success);
            NavigationManager.NavigateTo(BrandRegistersRouteFor.Index);
        }
        catch (Exception exception)
        {
            _ = Snackbar.Add(exception.Message, MudBlazor.Severity.Error);
        }
        finally
        {
            _isSubmitting = false;
        }
    }

    protected void GoBack()
    {
        NavigationManager.NavigateTo(BrandRegistersRouteFor.Index);
    }

    private static DialogOptions ConfirmationDialogOptions => new()
    {
        MaxWidth = MaxWidth.Small,
        FullWidth = true,
        CloseButton = true
    };

    private CreateBrandRegistrationRequest CreateRequest()
    {
        return new(Model.Brand ?? string.Empty, Model.ProductName ?? string.Empty, Model.Group ?? string.Empty,
            Model.Country ?? string.Empty, Model.Category ?? string.Empty, Model.ProductDescription ?? string.Empty);
    }

    private static IReadOnlyList<DocumentUploadItem> CreateDocuments(string prefix)
    {
        return
        [
            new($"{prefix}-mandatory-1", "Mandatory Document 1", true),
            new($"{prefix}-optional-1", "Optional Document 1", false),
            new($"{prefix}-mandatory-2", "Mandatory Document 2", true),
            new($"{prefix}-optional-2", "Optional Document 2", false),
            new($"{prefix}-mandatory-3", "Mandatory Document 3", true),
            new($"{prefix}-optional-3", "Optional Document 3", false),
            new($"{prefix}-mandatory-4", "Mandatory Document 4", true),
            new($"{prefix}-optional-4", "Optional Document 4", false)
        ];
    }

    protected sealed class BrandFormModel
    {
        public string? Brand { get; set; }
        public string? ProductName { get; set; }
        public string? Group { get; set; }
        public string? Country { get; set; }
        public string? Category { get; set; }
        public string? ProductDescription { get; set; }
    }

    protected sealed class DocumentUploadItem(string key, string label, bool isMandatory)
    {
        public string Key { get; } = key;
        public string Label { get; } = label;
        public bool IsMandatory { get; } = isMandatory;
        public string? FileName { get; set; }
    }
}
