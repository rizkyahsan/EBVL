namespace EBVL.Shared.Dto.Modules.MasterData.Lenders.GetLenders;

public sealed record LenderItem
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required string Address { get; init; }
    public required string Country { get; init; }
    public required Guid CountryId { get; init; }
    public required string PhoneNumber { get; init; }
    public required string FullPhoneNumber { get; init; }
    public required string EmailAddress { get; init; }
    public required string Website { get; init; }
}
