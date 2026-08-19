using EBVL.Shared.Dto.Modules.MasterData.Lenders.GetLender;

namespace EBVL.BackEnd.Logics.Modules.MasterData.Lenders.GetLender;

[AuthorizeRequest]
public sealed record GetLenderQuery : GetLenderRequest, IRequest<GetLenderResponse> { }

public sealed class GetLenderQueryValidator : AbstractValidatorBase<GetLenderQuery>
{
    public GetLenderQueryValidator()
    {
        Include(new GetLenderRequestValidator());
    }
}

public sealed class GetLenderQueryHandler(IDatabaseService databaseService)
    : IRequestHandler<GetLenderQuery, GetLenderResponse>
{
    public async Task<GetLenderResponse> Handle(GetLenderQuery request, CancellationToken cancellationToken)
    {
        var audits = await databaseService.Audits
            .Where(audit => audit.EntityName == nameof(Lender) && audit.EntityId == request.LenderId)
            .OrderBy(audit => audit.Created)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var lender = await databaseService.Lenders
            .Include(x => x.Country)
            .Where(x => !x.IsDeleted && x.Id == request.LenderId)
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
                Website = x.Website,
                Created = x.Created,
                CreatedBy = x.CreatedBy,
                Modified = x.Modified,
                ModifiedBy = x.ModifiedBy,
                Audits = audits.ToAuditItems<AuditItem>()
            })
            .SingleOrDefaultAsync(cancellationToken)
            ?? throw ExceptionFor.EntityNotFound(LendersDisplayTextFor.Lender, CommonDisplayTextFor.Id, request.LenderId);

        return new GetLenderResponse
        {
            Item = lender
        };
    }
}
