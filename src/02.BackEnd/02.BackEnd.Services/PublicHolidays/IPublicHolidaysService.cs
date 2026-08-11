namespace EBVL.BackEnd.Services.PublicHolidays;

public interface IPublicHolidaysService
{
    public Task<IEnumerable<PublicHolidayData>> GetPublicHolidaysAsync(int year, string countryCode, CancellationToken cancellationToken = default);
}
