using EBVL.Shared.Dto.Modules.MasterData;
using EBVL.Shared.Dto.Modules.MasterData.Countries.GetCountries;

namespace EBVL.BackEnd.Logics.Modules.MasterData.Countries.GetCountries;

[AuthorizeRequestByPermission(Permissions.MasterDataCountriesRead)]
public sealed record GetCountriesQuery : IRequest<GetCountriesResponse>
{
}

public sealed class GetCountriesQueryHandler(IDatabaseService databaseService)
    : IRequestHandler<GetCountriesQuery, GetCountriesResponse>
{
    public async Task<GetCountriesResponse> Handle(GetCountriesQuery request, CancellationToken cancellationToken)
    {
        var countries = await databaseService.Countries
            .Where(x => !x.IsDeleted)
            .OrderBy(x => x.Name)
            .Select(x => new CountryItem
            {
                Id = x.Id,
                Name = x.Name,
                Code = x.Code
            })
            .ToListAsync(cancellationToken);

        return new GetCountriesResponse
        {
            Items = countries
        };
    }
}
