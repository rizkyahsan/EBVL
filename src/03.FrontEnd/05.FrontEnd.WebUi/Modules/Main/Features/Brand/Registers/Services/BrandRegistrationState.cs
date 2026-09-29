namespace EBVL.FrontEnd.WebUi.Modules.Main.Features.Brand.Registers.Services;

public sealed class BrandRegistrationState
{
    private readonly List<BrandRegisterItem> _items =
    [
        new(Guid.NewGuid(), "Screwdriver", "Kitz", "Industrial Equipment", "Document Need Disposition", new(2026, 8, 19, 7, 0, 0, TimeSpan.Zero), new(2026, 8, 19, 7, 30, 0, TimeSpan.Zero)),
        new(Guid.NewGuid(), "Power Driller", "PowerShell", "Power Tools", "Review Document I", new(2026, 8, 19, 1, 0, 0, TimeSpan.Zero), new(2026, 8, 19, 7, 0, 0, TimeSpan.Zero)),
        new(Guid.NewGuid(), "Field Officer Uniform", "Textile Horizon", "Uniform", "Review Document II", new(2026, 8, 18, 6, 0, 0, TimeSpan.Zero), new(2026, 8, 19, 3, 0, 0, TimeSpan.Zero)),
        new(Guid.NewGuid(), "Bolt", "NEWCO", "Industrial Equipment", "Review Document III", new(2026, 8, 17, 8, 0, 0, TimeSpan.Zero), new(2026, 8, 18, 6, 0, 0, TimeSpan.Zero)),
        new(Guid.NewGuid(), "Drill", "De Wallt", "Power Tools", "Review by Admin Sr. Man MSAir", new(2026, 8, 7, 7, 0, 0, TimeSpan.Zero), new(2026, 8, 11, 7, 50, 0, TimeSpan.Zero)),
        new(Guid.NewGuid(), "Monitor", "Samsung", "Electronics", "Approved", new(2026, 8, 6, 8, 0, 0, TimeSpan.Zero), new(2026, 8, 8, 8, 0, 0, TimeSpan.Zero)),
        new(Guid.NewGuid(), "Valve", "CAT", "Industrial Equipment", "Approved", new(2026, 8, 3, 8, 0, 0, TimeSpan.Zero), new(2026, 8, 19, 8, 0, 0, TimeSpan.Zero)),
        new(Guid.NewGuid(), "Faucet", "TEKO", "Sanitary", "Approved", new(2026, 7, 2, 23, 0, 0, TimeSpan.Zero), new(2026, 7, 30, 10, 0, 0, TimeSpan.Zero)),
        new(Guid.NewGuid(), "Oil Faucet", "VALVE", "Sanitary", "Approved", new(2026, 6, 3, 10, 0, 0, TimeSpan.Zero), new(2026, 7, 30, 12, 0, 0, TimeSpan.Zero)),
        new(Guid.NewGuid(), "Software", "Claude", "Technology", "Reject by Admin MSAir", new(2026, 1, 3, 11, 0, 0, TimeSpan.Zero), new(2026, 7, 30, 12, 0, 0, TimeSpan.Zero))
    ];

    public IReadOnlyList<BrandRegisterItem> Items => _items;

    public BrandRegisterItem? Find(Guid id)
    {
        return _items.FirstOrDefault(item => item.Id == id);
    }

    public void Add(string? product, string? brand, string? group, string status)
    {
        var now = DateTimeOffset.UtcNow;
        _items.Insert(0, new BrandRegisterItem(
            Guid.NewGuid(),
            string.IsNullOrWhiteSpace(product) ? "-" : product.Trim(),
            string.IsNullOrWhiteSpace(brand) ? "-" : brand,
            string.IsNullOrWhiteSpace(group) ? "-" : group,
            status,
            now,
            now));
    }
}

public sealed record BrandRegisterItem(Guid Id, string Product, string Brand, string Group, string Status, DateTimeOffset SubmittedAt, DateTimeOffset LastUpdatedAt);
