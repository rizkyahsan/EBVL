using EBVL.Shared.Dto.Modules.Authentication.ExternalUsers.CheckExternalUser;
using EBVL.Shared.Statics.Common;

namespace EBVL.BackEnd.Logics.Modules.Authentication.ExternalUsers.CheckExternalUser;

public sealed record CheckExternalUserQuery : CheckExternalUserRequest, IRequest<CheckExternalUserResponse>
{
}

public sealed class CheckExternalUserQueryValidator : AbstractValidatorBase<CheckExternalUserQuery>
{
    public CheckExternalUserQueryValidator()
    {
        Include(new CheckExternalUserRequestValidator());
    }
}

public sealed class CheckExternalUserQueryHandler(IDatabaseService databaseService)
    : IRequestHandler<CheckExternalUserQuery, CheckExternalUserResponse>
{
    public async Task<CheckExternalUserResponse> Handle(CheckExternalUserQuery request, CancellationToken cancellationToken)
    {
        var now = TimeZoneInfo.ConvertTime(DateTimeOffset.Now, TimezoneFor.WibTimeZone);
        var lastValidLogin = await databaseService.ExternalLogins
            .Where(x => !x.IsDeleted && x.Id == request.ExternalLoginId && x.ExpiredAt >= now)
            .OrderByDescending(x => x.Created)
            .FirstOrDefaultAsync(cancellationToken);

        var isFound = lastValidLogin != null;
        return new CheckExternalUserResponse
        {
            Item = new UserItem
            {
                IsVerified = isFound
            }
        };
    }
}

