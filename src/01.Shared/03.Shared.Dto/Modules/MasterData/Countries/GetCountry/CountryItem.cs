namespace EBVL.Shared.Dto.Modules.MasterData.Countries.GetCountry;

public sealed record CountryItem
{
    public required Guid Id { get; init; }
    public required DateTimeOffset Created { get; init; }
    public required string CreatedBy { get; init; }
    public required DateTimeOffset? Modified { get; init; }
    public required string? ModifiedBy { get; init; }

    public required string Name { get; init; }
    public required string Code { get; init; }
    public required string PhoneCode { get; init; }
    public required string CurrencyCode { get; init; }
    public string Region { get; init; } = string.Empty;

    public required IEnumerable<AuditItem> Audits { get; init; }
}
