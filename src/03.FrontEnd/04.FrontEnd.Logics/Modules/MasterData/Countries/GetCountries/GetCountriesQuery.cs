using EBVL.Shared.Dto.Modules.MasterData.Countries.GetCountries;

namespace EBVL.FrontEnd.Logics.Modules.MasterData.Countries.GetCountries;

public sealed record GetCountriesQuery : IRequest<GetCountriesResponse>
{
}

public sealed class GetCountriesQueryHandler(IBackEndApiService backEndApiService)
    : IRequestHandler<GetCountriesQuery, GetCountriesResponse>
{
    public async Task<GetCountriesResponse> Handle(GetCountriesQuery request, CancellationToken cancellationToken)
    {
        var restRequest = new RestRequest(GetCountriesRoute.ResourceUri, Method.Get);

        return await backEndApiService.SendRequestAsync<GetCountriesResponse>(restRequest, cancellationToken);
    }
}
