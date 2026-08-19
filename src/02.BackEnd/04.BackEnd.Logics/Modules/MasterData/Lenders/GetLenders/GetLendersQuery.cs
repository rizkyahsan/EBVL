using EBVL.Shared.Dto.Modules.MasterData.Lenders.GetLenders;

namespace EBVL.BackEnd.Logics.Modules.MasterData.Lenders.GetLenders;

[AuthorizeRequest]
public sealed record GetLendersQuery : IRequest<GetLendersResponse> { }

public sealed class GetLendersQueryHandler(IDatabaseService databaseService)
    : IRequestHandler<GetLendersQuery, GetLendersResponse>
{
    public async Task<GetLendersResponse> Handle(GetLendersQuery request, CancellationToken cancellationToken)
    {
        var lenders = await databaseService.Lenders
            .Include(x => x.Country)
            .Where(x => !x.IsDeleted)
            .OrderBy(x => x.Name)
            .Select(x => new LenderItem
            {
                Id = x.Id,
                Name = x.Name,
                Address = x.Address,
                Country = x.Country.Name,
                CountryId = x.CountryId,
                PhoneNumber = x.PhoneNumber,
                FullPhoneNumber = x.FullPhoneNumber,
                EmailAddress = x.EmailAddress,
                Website = x.Website
            })
            .ToListAsync(cancellationToken);

        return new GetLendersResponse
        {
            Items = lenders
        };
    }
}
