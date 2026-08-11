namespace EBVL.Shared.Dto.Modules.Examples.PublicHolidays.GetPublicHolidays;

public sealed record PublicHolidayItem
{
    public required Guid Id { get; init; }
    public required DateOnly Date { get; set; }
    public required string Name { get; set; }
    public required string LocalName { get; set; }
    public required string CountryCode { get; set; }
}
