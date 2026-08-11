using Pertamina.Common.Statics;
using Pertamina.Services.CurrentUser;
using Pertamina.Services.Otp;
using EBVL.Shared.Dto.Modules.Main.Users.GetMyUser;

namespace EBVL.BackEnd.Logics.Modules.Main.Users.GetMyUser;

public sealed record GetMyUserQuery : IRequest<GetMyUserResponse>
{
}

public sealed class GetMyUserQueryHandler(
    IDatabaseService databaseService,
    ICurrentUserService currentUserService,
    IOtpService otpService)
    : IRequestHandler<GetMyUserQuery, GetMyUserResponse>
{
    public async Task<GetMyUserResponse> Handle(GetMyUserQuery request, CancellationToken cancellationToken)
    {
        var username = currentUserService.Username
            ?? throw ExceptionFor.NotAuthenticated();

        var user = await databaseService.Users
            .AsNoTracking()
            .Where(x => !x.IsDeleted && x.Username == username)
            .Select(x => new UserItem
            {
                Id = x.Id,
                Username = x.Username,
                Name = x.Name,
                EmailAddress = x.EmailAddress,
                PhoneNumber = x.PhoneNumber,
                QrCodeDataUri = $"data:{ContentTypeFor.ImagePng};base64,{Convert.ToBase64String(otpService.GetGraphic(x.OtpUrl))}",
                IsVerified = x.IsVerified
            })
            .SingleOrDefaultAsync(cancellationToken)
            ?? throw ExceptionFor.EntityNotFound(UsersDisplayTextFor.User, CommonDisplayTextFor.Username, username);

        return new GetMyUserResponse
        {
            Item = user
        };
    }
}
