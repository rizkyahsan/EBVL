using Pertamina.Services.CurrentUser;
using Pertamina.Services.Otp;
using Pertamina.Services.UserProfile;
using EBVL.Shared.Dto.Modules.Main.Users.CreateMyUser;

namespace EBVL.BackEnd.Logics.Modules.Main.Users.CreateMyUser;

[AuthorizeRequest]
public sealed record CreateMyUserCommand : IRequest<CreateMyUserResponse>
{
}

public sealed class CreateMyUserCommandHandler(
    IDatabaseService databaseService,
    ICurrentUserService currentUserService,
    IUserProfileService userProfileService,
    IOtpService otpService)
    : IRequestHandler<CreateMyUserCommand, CreateMyUserResponse>
{
    public async Task<CreateMyUserResponse> Handle(CreateMyUserCommand request, CancellationToken cancellationToken)
    {
        var username = currentUserService.Username
            ?? throw ExceptionFor.NotAuthenticated();

        var userExists = await databaseService.Users
            .AsNoTracking()
            .Where(x => !x.IsDeleted && x.Username == username)
            .AnyAsync(cancellationToken);

        if (userExists)
        {
            throw ExceptionFor.EntityAlreadyExists(UsersDisplayTextFor.User, CommonDisplayTextFor.Username, username);
        }

        var userProfileResponse = await userProfileService.GetUserProfileAsync(username, cancellationToken);
        var otp = otpService.CreateOtp(username);

        var user = new User
        {
            IdentityUserId = Guid.CreateVersion7(),
            LenderId = Guid.CreateVersion7(),
            Username = username,
            DisplayName = userProfileResponse.DisplayName,
            EmailAddress = userProfileResponse.EmailAddress,
            PhoneNumber = userProfileResponse.PhoneNumber,
            OtpSecret = otp.Secret,
            OtpUrl = otp.Url,
            IsVerified = false,
            IsPicLender = false
        };

        _ = await databaseService.Users.AddAsync(user, cancellationToken);
        _ = await databaseService.SaveAsync(nameof(CreateMyUser), cancellationToken);

        return new CreateMyUserResponse
        {
            Item = new UserItem
            {
                Id = user.Id,
                Username = user.Username
            }
        };
    }
}
