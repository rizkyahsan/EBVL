namespace EBVL.FrontEnd.WebUi.Modules.Main.Features.Brand.Registers.Pages;

public partial class Index
{
    private const string All = "ALL";
    private static readonly TimeZoneInfo _wibTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Asia/Jakarta");
    private readonly IReadOnlyList<BrandRegisterItem> _items =
    [
        new(1, "Screwdriver", "Kitz", "Industrial Equipment", "New", new(2026, 8, 19, 7, 0, 0, TimeSpan.Zero), new(2026, 8, 19, 7, 30, 0, TimeSpan.Zero)),
        new(2, "Power Driller", "PowerShell", "Power Tools", "Request Approval Analyst", new(2026, 8, 19, 1, 0, 0, TimeSpan.Zero), new(2026, 8, 19, 7, 0, 0, TimeSpan.Zero)),
        new(3, "Field Officer Uniform", "Textile Horizon", "Uniform", "Review by Admin Analyst", new(2026, 8, 18, 6, 0, 0, TimeSpan.Zero), new(2026, 8, 19, 3, 0, 0, TimeSpan.Zero)),
        new(4, "Bolt", "NEWCO", "Industrial Equipment", "Submitted", new(2026, 8, 17, 8, 0, 0, TimeSpan.Zero), new(2026, 8, 18, 6, 0, 0, TimeSpan.Zero)),
        new(5, "Drill", "De Wallt", "Power Tools", "Review by Admin Sr. Man MSAir", new(2026, 8, 7, 7, 0, 0, TimeSpan.Zero), new(2026, 8, 11, 7, 50, 0, TimeSpan.Zero)),
        new(6, "Monitor", "Samsung", "Electronics", "Approved", new(2026, 8, 6, 8, 0, 0, TimeSpan.Zero), new(2026, 8, 8, 8, 0, 0, TimeSpan.Zero)),
        new(7, "Valve", "CAT", "Industrial Equipment", "Approved", new(2026, 8, 3, 8, 0, 0, TimeSpan.Zero), new(2026, 8, 19, 8, 0, 0, TimeSpan.Zero)),
        new(8, "Faucet", "TEKO", "Sanitary", "Approved", new(2026, 7, 2, 23, 0, 0, TimeSpan.Zero), new(2026, 7, 30, 10, 0, 0, TimeSpan.Zero)),
        new(9, "Oil Faucet", "VALVE", "Sanitary", "Approved", new(2026, 6, 3, 10, 0, 0, TimeSpan.Zero), new(2026, 7, 30, 12, 0, 0, TimeSpan.Zero)),
        new(10, "Software", "Claude", "Technology", "Reject by Admin MSAir", new(2026, 1, 3, 11, 0, 0, TimeSpan.Zero), new(2026, 7, 30, 12, 0, 0, TimeSpan.Zero))
    ];
    private readonly string[] _statuses = [All, "New", "Submitted", "Request Approval Analyst", "Review by Admin Analyst", "Review by Admin Sr. Man MSAir", "Approved", "Reject by Admin MSAir"];
    private MudTable<BrandRegisterItem> _table = default!;
    private IReadOnlyList<BrandRegisterItem> _filteredItems = [];
    private string[] _brands = [];
    private string[] _groups = [];
    private string _brand = All;
    private string _group = All;
    private string _status = All;
    private string? _product;

    protected override void OnInitialized()
    {
        LoadBreadcrumbs();
        _brands = [All, .. _items.Select(item => item.Brand).Distinct(StringComparer.OrdinalIgnoreCase).OrderBy(value => value)];
        _groups = [All, .. _items.Select(item => item.Group).Distinct(StringComparer.OrdinalIgnoreCase).OrderBy(value => value)];
        _filteredItems = _items;
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

    private void SetBrand(string value)
    {
        _brand = value;
    }

    private void SetGroup(string value)
    {
        _group = value;
    }

    private void SetStatus(string value)
    {
        _status = value;
    }

    private void SetProduct(string value)
    {
        _product = value;
    }

    private void Search()
    {
        _filteredItems = [.. _items.Where(item => IsAllOrEqual(_brand, item.Brand)
            && IsAllOrEqual(_group, item.Group)
            && IsAllOrEqual(_status, item.Status)
            && (string.IsNullOrWhiteSpace(_product) || item.Product.Contains(_product.Trim(), StringComparison.OrdinalIgnoreCase)))];
        _table.NavigateTo(0);
    }

    private void RequestNewBrand()
    {
        _ = Snackbar.Add("Form Request New Brand akan ditambahkan pada tahap berikutnya.", MudBlazor.Severity.Info);
    }

    private void ViewBrand(BrandRegisterItem item)
    {
        _ = Snackbar.Add($"Detail brand {item.Brand} akan ditambahkan pada tahap berikutnya.", MudBlazor.Severity.Info);
    }

    private static bool IsAllOrEqual(string filter, string value)
    {
        return filter == All || string.Equals(filter, value, StringComparison.OrdinalIgnoreCase);
    }

    private static Color StatusColor(string status)
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

    private static string FormatWib(DateTimeOffset value)
    {
        return $"{TimeZoneInfo.ConvertTime(value, _wibTimeZone):yyyy-MM-dd HH:mm} WIB";
    }

    private sealed record BrandRegisterItem(int Number, string Product, string Brand, string Group, string Status, DateTimeOffset SubmittedAt, DateTimeOffset LastUpdatedAt);
}
