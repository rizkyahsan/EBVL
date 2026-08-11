using EBVL.Shared.Dto.Modules.MasterData;
using EBVL.Shared.Dto.Modules.MasterData.Countries.GetCountry;

namespace EBVL.BackEnd.Logics.Modules.MasterData.Countries.GetCountry;

[AuthorizeRequestByPermission(Permissions.MasterDataCountriesRead)]
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

public sealed class GetCountryQueryHandler(IDatabaseService databaseService)
    : IRequestHandler<GetCountryQuery, GetCountryResponse>
{
    public async Task<GetCountryResponse> Handle(GetCountryQuery request, CancellationToken cancellationToken)
    {
        var audits = await databaseService.Audits
            .Where(audit => audit.EntityName == nameof(Country) && audit.EntityId == request.CountryId)
            .OrderBy(audit => audit.Created)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var country = await databaseService.Countries
            .Where(x => !x.IsDeleted && x.Id == request.CountryId)
            .Select(x => new CountryItem
            {
                Id = x.Id,
                Name = x.Name,
                Code = x.Code,
                Created = x.Created,
                CreatedBy = x.CreatedBy,
                Modified = x.Modified,
                ModifiedBy = x.ModifiedBy,
                Audits = audits.ToAuditItems<AuditItem>()
            })
            .SingleOrDefaultAsync(cancellationToken)
            ?? throw ExceptionFor.EntityNotFound(CountriesDisplayTextFor.Country, CommonDisplayTextFor.Id, request.CountryId);

        return new GetCountryResponse
        {
            Item = country
        };
    }
}
