using EBVL.Shared.Dto.Modules.MasterData.Users.GetUser;

namespace EBVL.BackEnd.Logics.Modules.MasterData.Users.GetUser;

[AuthorizeRequest]
public sealed record GetUserQuery : GetUserRequest, IRequest<GetUserResponse>
{
}

public sealed class GetUserQueryValidator : AbstractValidatorBase<GetUserQuery>
{
    public GetUserQueryValidator()
    {
        Include(new GetUserRequestValidator());
    }
}

public sealed class GetUserQueryHandler(IDatabaseService databaseService)
    : IRequestHandler<GetUserQuery, GetUserResponse>
{
    public async Task<GetUserResponse> Handle(GetUserQuery request, CancellationToken cancellationToken)
    {
        var audits = await databaseService.Audits
            .Where(audit => audit.EntityName == nameof(User) && audit.EntityId == request.UserId)
            .OrderBy(audit => audit.Created)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var user = await databaseService.Users
            .Where(x => !x.IsDeleted && x.Id == request.UserId)
            .Select(x => new UserItem
            {
                Id = x.Id,
                LenderId = x.LenderId,
                LenderName = x.Lender.Name,
                Username = x.Username,
                Name = x.DisplayName,
                FullPhoneNumber = $"{x.PhoneCode} {x.PhoneNumber}",
                PhoneCode = x.PhoneCode ?? string.Empty,
                PhoneNumber = x.PhoneNumber ?? string.Empty,
                EmailAddress = x.EmailAddress,
                IsVerified = x.IsVerified,
                IsPicLender = x.IsPicLender,
                CountPicLender = databaseService.Users.Count(u =>
                    !u.IsDeleted && u.LenderId == x.LenderId && u.IsPicLender),
                Created = x.Created,
                CreatedBy = x.CreatedBy,
                Modified = x.Modified,
                ModifiedBy = x.ModifiedBy,
                Audits = audits.ToAuditItems<AuditItem>()
            })
            .SingleOrDefaultAsync(cancellationToken)
            ?? throw ExceptionFor.EntityNotFound(UsersDisplayTextFor.User, CommonDisplayTextFor.Id, request.UserId);

        return new GetUserResponse
        {
            Item = user
        };
    }
}
