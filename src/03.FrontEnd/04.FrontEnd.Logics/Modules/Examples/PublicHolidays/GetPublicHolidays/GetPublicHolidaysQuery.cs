using EBVL.Shared.Dto.Modules.Examples.PublicHolidays.GetPublicHolidays;

namespace EBVL.FrontEnd.Logics.Modules.Examples.PublicHolidays.GetPublicHolidays;

public sealed record GetPublicHolidaysQuery : IRequest<GetPublicHolidaysResponse>
{
}

public sealed class GetPublicHolidaysQueryHandler(IBackEndApiService backEndApiService)
    : IRequestHandler<GetPublicHolidaysQuery, GetPublicHolidaysResponse>
{
    public async Task<GetPublicHolidaysResponse> Handle(GetPublicHolidaysQuery request, CancellationToken cancellationToken)
    {
        var restRequest = new RestRequest(GetPublicHolidaysRoute.ResourceUri, Method.Get);

        return await backEndApiService.SendRequestAsync<GetPublicHolidaysResponse>(restRequest, cancellationToken);
    }
}
