using EBVL.FrontEnd.WebUi.Common.Components.Abstracts;
using EBVL.FrontEnd.WebUi.Modules.Main.Features.Brand.Registers.Components;
using EBVL.FrontEnd.WebUi.Modules.Main.Features.Brand.Registers.Services;

namespace EBVL.FrontEnd.WebUi.Modules.Main.Features.Brand.Registers.Pages;

public partial class Details : PageBase
{
    [Parameter]
    public Guid RegistrationId { get; set; }

    [Inject]
    public required BrandRegistrationState RegistrationState { get; init; }

    protected BrandRegisterItem? Item { get; private set; }
    protected List<ReviewItem> CompanyFields { get; private set; } = [];
    protected List<DocumentReviewItem> Documents { get; } = CreateDocuments();
    protected IReadOnlyList<ActivityItem> Activities { get; } =
    [
        new("Submitted", "Submitted by vendor@vendor.com", "19 Aug 2026, 13:00"),
        new("On Review", "On review by Admin MSAI", "19 Aug 2026, 13:00"),
        new("Approved", "Approved by Admin MSAI", "20 Aug 2026, 13:00")
    ];
    protected bool ShowValidationErrors { get; set; }
    protected string? SelectedSection { get; set; }
    protected string? ApprovalNotes { get; set; }

    protected bool IsDispositionStatus => Item?.Status == "Document Need Disposition";
    protected bool IsReviewStatus => Item?.Status is "Review Document I" or "Review Document II" or "Review Document III";
    protected string ReviewRole => Item?.Status switch
    {
        "Review Document I" => "Role Analis",
        "Review Document II" => "Role Senior Man",
        "Review Document III" => "Role Chief Section",
        _ => string.Empty
    };
    protected Color StatusColor => IsDispositionStatus || IsReviewStatus ? Color.Warning : Item?.Status == "Approved" ? Color.Success : Color.Info;

    protected override void OnInitialized()
    {
        Item = RegistrationState.Find(RegistrationId);
        LoadBreadcrumbs();
        if (Item is not null)
        {
            CompanyFields =
            [
                new("Brand", Item.Brand),
                new("Product Name", Item.Product),
                new("Group", Item.Group),
                new("COO / Factory Location", "Indonesia"),
                new("Category", "Goods"),
                new("Product Description", $"{Item.Product} product registration")
            ];
        }
    }

    protected override void LoadBreadcrumbs()
    {
        _breadcrumbItems = [MainBreadcrumbFor.Home, BrandRegistersBreadcrumbFor.Brand, new(BrandRegistersDisplayTextFor.Register, BrandRegistersRouteFor.Index), CommonBreadcrumbFor.Active(BrandRegistersDisplayTextFor.DetailBrand)];
    }

    protected void GoBack()
    {
        NavigationManager.NavigateTo(BrandRegistersRouteFor.Index);
    }

    protected void Preview(string fileName)
    {
        _ = Snackbar.Add($"Preview {fileName}.", MudBlazor.Severity.Info);
    }

    protected async Task OpenDisposition()
    {
        var dialog = await DialogService.ShowAsync<DialogDisposition>(string.Empty, new DialogOptions { MaxWidth = MaxWidth.Large, FullWidth = true, CloseButton = true });
        var result = await dialog.Result;
        if (result is not null && !result.Canceled)
        {
            _ = Snackbar.Add("Brand berhasil didisposisikan.", MudBlazor.Severity.Success);
        }
    }

    protected void Decide(string decision)
    {
        ShowValidationErrors = true;
        if (string.IsNullOrWhiteSpace(SelectedSection) || CompanyFields.Concat<ReviewItem>(Documents).Any(item => item.IsValid is null || (item.IsValid is false && string.IsNullOrWhiteSpace(item.Remark))))
        {
            _ = Snackbar.Add("Lengkapi penilaian dokumen dan section sebelum melanjutkan.", MudBlazor.Severity.Error);
            return;
        }

        _ = Snackbar.Add($"Brand registration {decision}.", MudBlazor.Severity.Success);
    }

    private static List<DocumentReviewItem> CreateDocuments()
    {
        return [.. Enumerable.Range(1, 4).Select(index => new DocumentReviewItem($"Mandatory Document {index}", "Administrative", true)), .. Enumerable.Range(1, 4).Select(index => new DocumentReviewItem($"Optional Document {index}", "Technical", false))];
    }

    protected class ReviewItem(string label, string value)
    {
        public string Label { get; } = label;
        public string Value { get; } = value;
        public bool? IsValid { get; set; } = true;
        public string? Remark { get; set; }
    }

    protected sealed class DocumentReviewItem(string name, string category, bool isMandatory) : ReviewItem(name, "Uploaded document")
    {
        public string Name { get; } = name;
        public string Category { get; } = category;
        public bool IsMandatory { get; } = isMandatory;
        public string FileName { get; } = "letter.pdf";
    }

    protected sealed record ActivityItem(string Title, string Description, string OccurredAt);
}
