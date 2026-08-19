using EBVL.BackEnd.Services.LocalIdentity;
using EBVL.Shared.Dto.Modules.Authentication.ExternalUsers.VerifiedExternalUser;
using EBVL.Shared.Statics.Common;
using Pertamina.Services.Otp;

namespace EBVL.BackEnd.Logics.Modules.Authentication.ExternalUsers.VerifiedExternalUser;

public sealed record VerifiedExternalUserCommand : VerifiedExternalUserRequest, IRequest<VerifiedExternalUserResponse> { }

public sealed class VerifiedExternalUserCommandValidator : AbstractValidatorBase<VerifiedExternalUserCommand>
{
    public VerifiedExternalUserCommandValidator()
    {
        Include(new VerifiedExternalUserRequestValidator());
    }
}

public sealed class CheckVerificationUserQueryHandler(ILocalIdentityService localIdentityService,
    IDatabaseService databaseService,
    IOtpService otpService)
    : IRequestHandler<VerifiedExternalUserCommand, VerifiedExternalUserResponse>
{
    public async Task<VerifiedExternalUserResponse> Handle(VerifiedExternalUserCommand request, CancellationToken cancellationToken)
    {
        var now = TimeZoneInfo.ConvertTime(DateTimeOffset.Now, TimezoneFor.WibTimeZone);
        var externalLogin = await databaseService.ExternalLogins
            .Include(x => x.ExternalLoginLog)
            .Where(x => !x.IsDeleted && x.Id == request.ExternalLoginId && !x.IsUsed && x.ExpiredAt > now)
            .SingleOrDefaultAsync(cancellationToken)
            ?? throw new InvalidOperationException($"OTP session expired.");

        var user = await databaseService.Users
            .Where(x => !x.IsDeleted && x.Id == externalLogin.UserId)
            .SingleOrDefaultAsync(cancellationToken)
            ?? throw new InvalidOperationException($"OTP session expired.");

        var verificationCodeIsValid = otpService.VerifyCode(user.OtpSecret!, request.VerificationCode);

        if (!verificationCodeIsValid)
        {
            externalLogin.ExternalLoginLog.FailureReason = $"Invalid OTP.";

            _ = await databaseService.SaveAsync(nameof(VerifiedExternalUser), cancellationToken);

            throw new InvalidOperationException($"Invalid OTP.");
        }

        var verifiedAt = TimeZoneInfo.ConvertTime(DateTimeOffset.Now, TimezoneFor.WibTimeZone);
        externalLogin.IsUsed = true;
        externalLogin.ExternalLoginLog.IsSuccess = true;
        externalLogin.ExternalLoginLog.VerifiedAt = verifiedAt;

        _ = await databaseService.SaveAsync(nameof(VerifiedExternalUser), cancellationToken);

        var result = await localIdentityService.LoginAsync(user.Username);

        if (!result.Succeeded)
        {
            return new VerifiedExternalUserResponse
            {
                Item = new VerifiedExternalUserResult
                {
                    Succeeded = false,
                    ErrorMessage = result.ErrorMessage,
                    UserToken = null
                }
            };
        }

        var userToken = localIdentityService.GenerateToken(result.Claims);

        return new VerifiedExternalUserResponse
        {
            Item = new VerifiedExternalUserResult
            {
                Succeeded = true,
                ErrorMessage = result.ErrorMessage,
                UserToken = userToken
            }
        };
    }
}
