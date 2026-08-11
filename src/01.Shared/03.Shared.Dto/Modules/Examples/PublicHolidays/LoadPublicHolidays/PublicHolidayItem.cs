namespace EBVL.Shared.Dto.Modules.Examples.PublicHolidays.LoadPublicHolidays;

public sealed record PublicHolidayItem
{
    public required DateOnly Date { get; set; }
    public required string Name { get; set; }
    public required string LocalName { get; set; }
    public required string CountryCode { get; set; }
}
