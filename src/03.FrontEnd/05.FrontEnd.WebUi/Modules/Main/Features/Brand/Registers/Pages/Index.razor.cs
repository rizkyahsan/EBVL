using EBVL.FrontEnd.WebUi.Common.Components.Abstracts;
using EBVL.FrontEnd.WebUi.Modules.Main.Features.Brand.Registers.Services;

namespace EBVL.FrontEnd.WebUi.Modules.Main.Features.Brand.Registers.Pages;

public partial class Index : PageBase
{
    private const string All = "ALL";
    private static readonly TimeZoneInfo _wibTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Asia/Jakarta");
    [Inject]
    public required BrandRegistrationState RegistrationState { get; init; }

    protected IReadOnlyList<string> Statuses { get; } = [All, "Draft", "New", "Submitted", "Request Approval Analyst", "Review by Admin Analyst", "Review by Admin Sr. Man MSAir", "Approved", "Reject by Admin MSAir"];
    protected IReadOnlyList<BrandRegisterItem> FilteredItems { get; set; } = [];
    protected IReadOnlyList<string> Brands { get; set; } = [];
    protected IReadOnlyList<string> Groups { get; set; } = [];
    private string _brand = All;
    private string _group = All;
    private string _status = All;
    private string? _product;

    protected override void OnInitialized()
    {
        LoadBreadcrumbs();
        Brands = [All, .. RegistrationState.Items.Select(item => item.Brand).Distinct(StringComparer.OrdinalIgnoreCase).OrderBy(value => value)];
        Groups = [All, .. RegistrationState.Items.Select(item => item.Group).Distinct(StringComparer.OrdinalIgnoreCase).OrderBy(value => value)];
        FilteredItems = RegistrationState.Items;
    }

    protected override void LoadBreadcrumbs()
    {
        _breadcrumbItems =
        [
            MainBreadcrumbFor.Home,
            BrandRegistersBreadcrumbFor.Brand,
            CommonBreadcrumbFor.Active(BrandRegistersDisplayTextFor.Register)
        ];
    }

    protected void SetBrand(string value)
    {
        _brand = value;
    }

    protected void SetGroup(string value)
    {
        _group = value;
    }

    protected void SetStatus(string value)
    {
        _status = value;
    }

    protected void SetProduct(string value)
    {
        _product = value;
    }

    protected void Search()
    {
        FilteredItems = [.. RegistrationState.Items.Where(item => IsAllOrEqual(_brand, item.Brand)
            && IsAllOrEqual(_group, item.Group)
            && IsAllOrEqual(_status, item.Status)
            && (string.IsNullOrWhiteSpace(_product) || item.Product.Contains(_product.Trim(), StringComparison.OrdinalIgnoreCase)))];
    }

    protected void RequestNewBrand()
    {
        NavigationManager.NavigateTo(BrandRegistersRouteFor.Create);
    }

    protected void ViewBrand(BrandRegisterItem item)
    {
        _ = Snackbar.Add($"Detail brand {item.Brand} akan ditambahkan pada tahap berikutnya.", MudBlazor.Severity.Info);
    }

    protected int Number(BrandRegisterItem item)
    {
        return FilteredItems.ToList().IndexOf(item) + 1;
    }

    private static bool IsAllOrEqual(string filter, string value)
    {
        return filter == All || string.Equals(filter, value, StringComparison.OrdinalIgnoreCase);
    }

    protected static Color StatusColor(string status)
    {
        if (status.Contains("Reject", StringComparison.OrdinalIgnoreCase))
        {
            return Color.Error;
        }

        if (status.Equals("Approved", StringComparison.OrdinalIgnoreCase))
        {
            return Color.Success;
        }

        return status.Equals("New", StringComparison.OrdinalIgnoreCase) ? Color.Info : Color.Warning;
    }

    protected static string FormatWib(DateTimeOffset value)
    {
        return $"{TimeZoneInfo.ConvertTime(value, _wibTimeZone):yyyy-MM-dd HH:mm} WIB";
    }
}
