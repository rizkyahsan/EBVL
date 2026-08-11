using Pertamina.Services.CurrentUser;
using Pertamina.Services.Otp;
using Pertamina.Services.UserProfile;
using EBVL.Shared.Dto.Modules.Main.Users.ReloadMyUser;

namespace EBVL.BackEnd.Logics.Modules.Main.Users.ReloadMyUser;

public sealed record ReloadMyUserCommand : ReloadMyUserRequest, IRequest
{
}

public sealed class ReloadMyUserCommandValidator : AbstractValidator<ReloadMyUserCommand>
{
    public ReloadMyUserCommandValidator()
    {
        Include(new ReloadMyUserRequestValidator());
    }
}

public sealed class ReloadMyUserCommandHandler(
    IDatabaseService databaseService,
    ICurrentUserService currentUserService,
    IUserProfileService userProfileService,
    IOtpService otpService)
    : IRequestHandler<ReloadMyUserCommand>
{
    public async Task Handle(ReloadMyUserCommand request, CancellationToken cancellationToken)
    {
        var username = currentUserService.Username
            ?? throw ExceptionFor.NotAuthenticated();

        var user = await databaseService.Users
            .Where(x => !x.IsDeleted && x.Username == username)
            .SingleOrDefaultAsync(cancellationToken)
            ?? throw ExceptionFor.EntityNotFound(UsersDisplayTextFor.User, CommonDisplayTextFor.Username, username);

        if (!user.IsVerified)
        {
            throw new InvalidOperationException($"{UsersDisplayTextFor.User} {user.Username} is not verified.");
        }

        var verificationCodeIsValid = otpService.VerifyCode(user.OtpSecret, request.VerificationCode);

        if (!verificationCodeIsValid)
        {
            throw new InvalidOperationException($"{CommonDisplayTextFor.VerificationCode} is invalid.");
        }

        var userProfileResponse = await userProfileService.GetUserProfileAsync(username, cancellationToken);

        user.Name = userProfileResponse.DisplayName;
        user.EmailAddress = userProfileResponse.EmailAddress;
        user.PhoneNumber = userProfileResponse.PhoneNumber;

        _ = await databaseService.SaveAsync(nameof(ReloadMyUser), cancellationToken);
    }
}
