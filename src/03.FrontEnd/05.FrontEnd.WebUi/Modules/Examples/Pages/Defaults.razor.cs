namespace EBVL.FrontEnd.WebUi.Modules.Examples.Pages;

public partial class Defaults
{
    private const int ProductsCount = 33;
    private IEnumerable<ProductItem> _products = [];
    private string? _searchKeyword;

    protected override void OnInitialized()
    {
        LoadBreadcrumbs();

        _products = Enumerable.Range(1, ProductsCount).Select(i => new ProductItem
        {
            Name = $"Product {i}",
            UnitPrice = Math.Round(Convert.ToDecimal(Random.Shared.Next(1, 1000)), 2),
            Stock = Random.Shared.Next(0, 100)
        }).ToList();
    }

    protected override void LoadBreadcrumbs()
    {
        _breadcrumbItems =
        [
            MainBreadcrumbFor.Home,
            ExamplesBreadcrumbFor.Index,
            CommonBreadcrumbFor.Active(ExamplesDisplayTextFor.Defaults)
        ];
    }

    private bool FilterItems(ProductItem item)
    {
        if (string.IsNullOrWhiteSpace(_searchKeyword))
        {
            return true;
        }

        if (item.Name.Contains(_searchKeyword, StringComparison.InvariantCultureIgnoreCase))
        {
            return true;
        }

        if (item.UnitPrice.ToDisplayText(CurrencyFormatFor.NoDecimal).Contains(_searchKeyword, StringComparison.InvariantCultureIgnoreCase))
        {
            return true;
        }

        if (item.Stock.ToString().Contains(_searchKeyword, StringComparison.InvariantCultureIgnoreCase))
        {
            return true;
        }

        return false;
    }

    public sealed record ProductItem
    {
        public required string Name { get; init; }
        public required decimal UnitPrice { get; init; }
        public required int Stock { get; init; }
    }
}
