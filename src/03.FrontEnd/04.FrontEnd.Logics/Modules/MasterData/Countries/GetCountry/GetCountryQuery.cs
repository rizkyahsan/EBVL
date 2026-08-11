using EBVL.Shared.Dto.Modules.MasterData.Countries.GetCountry;

namespace EBVL.FrontEnd.Logics.Modules.MasterData.Countries.GetCountry;

public sealed record GetCountryQuery : GetCountryRequest, IRequest<GetCountryResponse>
{
}

public sealed class GetCountryQueryValidator : AbstractValidatorBase<GetCountryQuery>
{
    public GetCountryQueryValidator()
    {
        Include(new GetCountryRequestValidator());
    }
}

public sealed class GetCountryQueryHandler(IBackEndApiService backEndApiService)
    : IRequestHandler<GetCountryQuery, GetCountryResponse>
{
    public async Task<GetCountryResponse> Handle(GetCountryQuery request, CancellationToken cancellationToken)
    {
        var restRequest = new RestRequest(GetCountryRoute.ResourceUri(request.CountryId), Method.Get);

        return await backEndApiService.SendRequestAsync<GetCountryResponse>(restRequest, cancellationToken);
    }
}
