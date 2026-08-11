using Pertamina.Services.CurrentUser;
using Pertamina.Services.Otp;
using EBVL.Shared.Dto.Modules.Main.Users.VerifyMyUser;

namespace EBVL.BackEnd.Logics.Modules.Main.Users.VerifyMyUser;

public sealed record VerifyMyUserCommand : VerifyMyUserRequest, IRequest
{
}

public sealed class VerifyMyUserCommandValidator : AbstractValidator<VerifyMyUserCommand>
{
    public VerifyMyUserCommandValidator()
    {
        Include(new VerifyMyUserRequestValidator());
    }
}

public sealed class VerifyMyUserCommandHandler(
    IDatabaseService databaseService,
    ICurrentUserService currentUserService,
    IOtpService otpService)
    : IRequestHandler<VerifyMyUserCommand>
{
    public async Task Handle(VerifyMyUserCommand request, CancellationToken cancellationToken)
    {
        var username = currentUserService.Username
            ?? throw ExceptionFor.NotAuthenticated();

        var user = await databaseService.Users
            .Where(x => !x.IsDeleted && x.Username == username)
            .SingleOrDefaultAsync(cancellationToken)
            ?? throw ExceptionFor.EntityNotFound(UsersDisplayTextFor.User, CommonDisplayTextFor.Username, username);

        if (user.IsVerified)
        {
            throw new InvalidOperationException($"{UsersDisplayTextFor.User} {user.Username} is already verified.");
        }

        var verificationCodeIsValid = otpService.VerifyCode(user.OtpSecret, request.VerificationCode);

        if (!verificationCodeIsValid)
        {
            throw new InvalidOperationException($"{CommonDisplayTextFor.VerificationCode} is invalid.");
        }

        user.IsVerified = true;

        _ = await databaseService.SaveAsync(nameof(VerifyMyUser), cancellationToken);
    }
}
