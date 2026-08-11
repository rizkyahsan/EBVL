namespace EBVL.BackEnd.Infrastructure.PublicHolidays;

public sealed record PublicHolidaysOptions
{
    public const string SectionKey = nameof(PublicHolidays);

    public required string BaseUrl { get; init; }
}
