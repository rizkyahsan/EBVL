using EBVL.BackEnd.Services.LocalIdentity;
using EBVL.Shared.Dto.Modules.Authentication.ExternalUsers.LoginExternalUser;
using EBVL.Shared.Statics.Common;

namespace EBVL.BackEnd.Logics.Modules.Authentication.ExternalUsers.LoginExternalUser;

public sealed record LoginExternalUserCommand : LoginExternalUserRequest, IRequest<LoginExternalUserResponse>
{
}

public sealed class LoginExternalUserCommandValidator : AbstractValidatorBase<LoginExternalUserCommand>
{
    public LoginExternalUserCommandValidator()
    {
        Include(new LoginExternalUserRequestValidator());
    }
}

public sealed class LoginExternalUserCommandHandler(IDatabaseService databaseService,
    ILocalIdentityService localIdentityService)
    : IRequestHandler<LoginExternalUserCommand, LoginExternalUserResponse>
{
    public async Task<LoginExternalUserResponse> Handle(LoginExternalUserCommand request, CancellationToken cancellationToken)
    {
        var attemptedAt = TimeZoneInfo.ConvertTime(DateTimeOffset.Now, TimezoneFor.WibTimeZone);
        var loginLog = new ExternalLogin
        {
            Username = request.Username,
            IsSuccess = false,
            AttemptedAt = attemptedAt
        };

        _ = await databaseService.ExternalLogins.AddAsync(loginLog, cancellationToken);
        _ = await databaseService.SaveAsync(nameof(LoginExternalUser), cancellationToken);

        var result = await localIdentityService.VerifyUserPasswordAsync(request.Username, request.Password);

        if (!result.Succeeded)
        {
            loginLog.FailureReason = result.ErrorMessage;
            _ = await databaseService.SaveAsync(nameof(LoginExternalUser), cancellationToken);

            return new LoginExternalUserResponse
            {
                Item = new LoginExternalUserResult
                {
                    Succeeded = false,
                    ErrorMessage = "Invalid username or password"
                }
            };
        }

        var identityUserId = result.IdentityUserId;

        var user = await databaseService.Users
            .AsNoTracking()
            .Include(x => x.Lender)
            .FirstOrDefaultAsync(x => !x.IsDeleted
                && x.IsVerified
                && !x.Lender.IsDeleted
                && x.IdentityUserId == identityUserId, cancellationToken);

        if (user is null)
        {
            loginLog.FailureReason = "User Id Not Found";
            _ = await databaseService.SaveAsync(nameof(LoginExternalUser), cancellationToken);

            return new LoginExternalUserResponse
            {
                Item = new LoginExternalUserResult
                {
                    Succeeded = false,
                    ErrorMessage = "Invalid username or password"
                }
            };
        }

        loginLog.UserId = user.Id;

        var loginResult = await localIdentityService.LoginAsync(user.Username);
        if (!loginResult.Succeeded)
        {
            loginLog.FailureReason = loginResult.ErrorMessage;
            _ = await databaseService.SaveAsync(nameof(LoginExternalUser), cancellationToken);

            return new LoginExternalUserResponse
            {
                Item = new LoginExternalUserResult
                {
                    Succeeded = false,
                    ErrorMessage = "Invalid username or password"
                }
            };
        }

        loginLog.IsSuccess = true;
        loginLog.VerifiedAt = attemptedAt;
        _ = await databaseService.SaveAsync(nameof(LoginExternalUser), cancellationToken);

        return new LoginExternalUserResponse
        {
            Item = new LoginExternalUserResult
            {
                Succeeded = true,
                RequireOtp = false,
                UserToken = localIdentityService.GenerateToken(loginResult.Claims)
            }
        };
    }
}
