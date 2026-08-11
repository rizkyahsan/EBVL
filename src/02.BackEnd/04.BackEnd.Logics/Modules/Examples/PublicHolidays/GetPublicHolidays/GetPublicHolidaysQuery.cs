using EBVL.Shared.Dto.Modules.Examples.PublicHolidays.GetPublicHolidays;

namespace EBVL.BackEnd.Logics.Modules.Examples.PublicHolidays.GetPublicHolidays;

public sealed record GetPublicHolidaysQuery : IRequest<GetPublicHolidaysResponse>
{
}

public sealed class GetPublicHolidaysQueryHandler(IDatabaseService databaseService)
    : IRequestHandler<GetPublicHolidaysQuery, GetPublicHolidaysResponse>
{
    public async Task<GetPublicHolidaysResponse> Handle(GetPublicHolidaysQuery request, CancellationToken cancellationToken)
    {
        var publicHolidays = await databaseService.PublicHolidays
            .Where(x => !x.IsDeleted)
            .OrderBy(x => x.Name)
            .Select(x => new PublicHolidayItem
            {
                Id = x.Id,
                Date = x.Date,
                Name = x.Name,
                LocalName = x.LocalName,
                CountryCode = x.CountryCode
            })
            .ToListAsync(cancellationToken);

        return new GetPublicHolidaysResponse
        {
            Items = publicHolidays
        };
    }
}
