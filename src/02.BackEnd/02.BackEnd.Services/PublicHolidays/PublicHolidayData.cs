namespace EBVL.BackEnd.Services.PublicHolidays;

public sealed record PublicHolidayData
{
    public required string CountryCode { get; set; }
    public required DateOnly Date { get; set; }
    public required string Name { get; set; }
    public required string LocalName { get; set; }
}
