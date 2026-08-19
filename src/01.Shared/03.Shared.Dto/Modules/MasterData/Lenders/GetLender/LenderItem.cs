namespace EBVL.Shared.Dto.Modules.MasterData.Lenders.GetLender;

public sealed record LenderItem
{
    public required Guid Id { get; init; }
    public required DateTimeOffset Created { get; init; }
    public required string CreatedBy { get; init; }
    public required DateTimeOffset? Modified { get; init; }
    public required string? ModifiedBy { get; init; }

    public required string Name { get; init; }
    public required string Address { get; init; }
    public required string Country { get; init; }
    public required Guid CountryId { get; init; }
    public required string PhoneNumber { get; init; }
    public required string FullPhoneNumber { get; init; }
    public required string EmailAddress { get; init; }
    public required string Website { get; init; }

    public required IEnumerable<AuditItem> Audits { get; init; }
}
