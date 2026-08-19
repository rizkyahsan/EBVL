namespace EBVL.Shared.Dto.Modules.MasterData.Countries.GetCountries;

public sealed record CountryItem
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required string Code { get; init; }
    public required string PhoneCode { get; set; }
    public required string CurrencyCode { get; init; }
    public string Region { get; init; } = string.Empty;
}
