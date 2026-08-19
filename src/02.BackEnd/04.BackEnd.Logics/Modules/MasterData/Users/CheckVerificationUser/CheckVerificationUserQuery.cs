using EBVL.BackEnd.Services.LocalIdentity;
using EBVL.Shared.Dto.Modules.MasterData.Users.CheckVerificationUser;

namespace EBVL.BackEnd.Logics.Modules.MasterData.Users.CheckVerificationUser;

public sealed record CheckVerificationUserQuery : CheckVerificationUserRequest, IRequest<CheckVerificationUserResponse>
{
}

public sealed class CheckVerificationUserQueryValidator : AbstractValidatorBase<CheckVerificationUserQuery>
{
    public CheckVerificationUserQueryValidator()
    {
        Include(new CheckVerificationUserRequestValidator());
    }
}

public sealed class CheckVerificationUserQueryHandler(ILocalIdentityService localIdentityService,
    IDatabaseService databaseService)
    : IRequestHandler<CheckVerificationUserQuery, CheckVerificationUserResponse>
{
    public async Task<CheckVerificationUserResponse> Handle(CheckVerificationUserQuery request, CancellationToken cancellationToken)
    {
        var isValid = await localIdentityService.CheckAccessAsync(request.UserId, request.Token);

        if (isValid)
        {
            var user = await databaseService.Users
                .Where(x => !x.IsDeleted && x.Id == request.UserId)
                .SingleOrDefaultAsync(cancellationToken)
                ?? throw ExceptionFor.EntityNotFound(UsersDisplayTextFor.User, CommonDisplayTextFor.UserId, request.UserId);

            return new CheckVerificationUserResponse
            {
                Item = new UserItem
                {
                    Id = request.UserId,
                    Username = user.Username,
                    IsVerified = isValid
                }
            };
        }
        else
        {
            return new CheckVerificationUserResponse
            {
                Item = new UserItem
                {
                    Id = request.UserId,
                    Username = string.Empty,
                    IsVerified = isValid
                }
            };
        }
    }
}
