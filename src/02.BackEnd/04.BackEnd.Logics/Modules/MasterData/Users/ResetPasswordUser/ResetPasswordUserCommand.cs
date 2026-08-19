using EBVL.BackEnd.Services.LocalIdentity;
using EBVL.Shared.Dto.Modules.MasterData.Users.ResetPasswordUser;
using Pertamina.Services.Otp;

namespace EBVL.BackEnd.Logics.Modules.MasterData.Users.ResetPasswordUser;

public sealed record ResetPasswordUserCommand : ResetPasswordUserRequest, IRequest<ResetPasswordUserResponse>
{
}

public sealed class ResetPasswordUserCommandValidator : AbstractValidatorBase<ResetPasswordUserCommand>
{
    public ResetPasswordUserCommandValidator()
    {
        Include(new ResetPasswordUserRequestValidator());
    }
}

public sealed class ResetPasswordUserCommandHandler(IDatabaseService databaseService,
    ILocalIdentityService localIdentityService,
    IOtpService otpService)
    : IRequestHandler<ResetPasswordUserCommand, ResetPasswordUserResponse>
{
    public async Task<ResetPasswordUserResponse> Handle(ResetPasswordUserCommand request, CancellationToken cancellationToken)
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

        var verificationCodeIsValid = otpService.VerifyCode(user.OtpSecret!, request.VerificationCode);

        if (!verificationCodeIsValid)
        {
            throw new InvalidOperationException($"Otp failed.");
        }

        var verifiedUserId = await localIdentityService.VerifyUserAsync(user.IdentityUserId);
        var changePassUserId = await localIdentityService.UpdatePasswordAsync(user.IdentityUserId, request.Password);

        user.AccessTokenHash = null;
        user.AccessTokenExpiredAt = null;

        _ = await databaseService.SaveAsync(nameof(ResetPasswordUser), cancellationToken);

        return new ResetPasswordUserResponse
        {
            Item = new ResetPasswordUserResult
            {
                Succeeded = true,
                ErrorMessage = null
            }
        };
    }
}
