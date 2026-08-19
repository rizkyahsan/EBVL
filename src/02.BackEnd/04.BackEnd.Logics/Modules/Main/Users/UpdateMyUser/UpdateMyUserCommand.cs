using Pertamina.Services.CurrentUser;
using Pertamina.Services.Otp;
using EBVL.Shared.Dto.Modules.Main.Users.UpdateMyUser;

namespace EBVL.BackEnd.Logics.Modules.Main.Users.UpdateMyUser;

[AuthorizeRequest]
public sealed record UpdateMyUserCommand : UpdateMyUserRequest, IRequest
{
}

public sealed class UpdateMyUserCommandValidator : AbstractValidatorBase<UpdateMyUserCommand>
{
    public UpdateMyUserCommandValidator()
    {
        Include(new UpdateMyUserRequestValidator());
    }
}

public sealed class UpdateMyUserCommandHandler(
    IDatabaseService databaseService,
    ICurrentUserService currentUserService,
    IOtpService otpService)
    : IRequestHandler<UpdateMyUserCommand>
{
    public async Task Handle(UpdateMyUserCommand request, CancellationToken cancellationToken)
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

        var verificationCodeIsValid = otpService.VerifyCode(user.OtpSecret!, request.VerificationCode);

        if (!verificationCodeIsValid)
        {
            throw new InvalidOperationException($"{CommonDisplayTextFor.VerificationCode} is invalid.");
        }

        user.DisplayName = request.Name;
        user.EmailAddress = request.EmailAddress;
        user.PhoneNumber = request.PhoneNumber;

        _ = await databaseService.SaveAsync(nameof(VerifyMyUser), cancellationToken);
    }
}
