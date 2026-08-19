using EBVL.BackEnd.Services.LocalIdentity;
using EBVL.Shared.Dto.Modules.MasterData.Users.VerifiedUser;
using Pertamina.Services.Otp;

namespace EBVL.BackEnd.Logics.Modules.MasterData.Users.VerifiedUser;

public sealed record VerifiedUserCommand : VerifiedUserRequest, IRequest<VerifiedUserResponse>
{
}

public sealed class VerifiedUserCommandValidator : AbstractValidatorBase<VerifiedUserCommand>
{
    public VerifiedUserCommandValidator()
    {
        Include(new VerifiedUserRequestValidator());
    }
}

public sealed class VerifiedUserCommandHandler(IDatabaseService databaseService,
    ILocalIdentityService localIdentityService,
    IOtpService otpService)
    : IRequestHandler<VerifiedUserCommand, VerifiedUserResponse>
{
    public async Task<VerifiedUserResponse> Handle(VerifiedUserCommand request, CancellationToken cancellationToken)
    {
        var isValid = await localIdentityService.CheckAccessAsync(request.UserId, request.Token);

        if (!isValid)
        {
            throw new InvalidOperationException($"{CommonDisplayTextFor.AccessDenied}!");
        }

        var user = await databaseService.Users
            .Where(x => !x.IsDeleted && x.Id == request.UserId)
            .SingleOrDefaultAsync(cancellationToken)
            ?? throw ExceptionFor.EntityNotFound(UsersDisplayTextFor.User, CommonDisplayTextFor.Username, request.UserId);

        if (user.IsVerified)
        {
            throw new InvalidOperationException($"Verification failed.");
        }

        var verificationCodeIsValid = otpService.VerifyCode(user.OtpSecret!, request.VerificationCode);

        if (!verificationCodeIsValid)
        {
            throw new InvalidOperationException($"Otp failed.");
        }

        var verifiedUserId = await localIdentityService.VerifyUserAsync(user.IdentityUserId);
        var changePassUserId = await localIdentityService.UpdatePasswordAsync(user.IdentityUserId, request.Password);

        user.IsVerified = true;
        user.AccessTokenHash = null;
        user.AccessTokenExpiredAt = null;

        _ = await databaseService.SaveAsync(nameof(VerifiedUser), cancellationToken);

        return new VerifiedUserResponse
        {
            Item = new VerifiedUserResult
            {
                Succeeded = true,
                ErrorMessage = null
            }
        };
    }
}
