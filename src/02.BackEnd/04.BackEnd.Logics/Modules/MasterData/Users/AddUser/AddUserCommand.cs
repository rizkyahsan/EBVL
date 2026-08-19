using System.Security.Cryptography;
using System.Text;
using EBVL.BackEnd.Logics.Common.Builder;
using EBVL.BackEnd.Services.AppConfigBackEnd;
using EBVL.BackEnd.Services.EmailBlast2;
using EBVL.BackEnd.Services.EmailBlast2.Model;
using EBVL.BackEnd.Services.LocalIdentity;
using EBVL.Shared.Dto.Modules.MasterData.Users.AddUser;
using EBVL.Shared.Statics.Common;
using EBVL.Shared.Statics.Configurations;
using Microsoft.Extensions.Options;
using Pertamina.Services.CurrentUser;
using Pertamina.Services.Otp;

namespace EBVL.BackEnd.Logics.Modules.MasterData.Users.AddUser;

[AuthorizeRequest]
public sealed record AddUserCommand : AddUserRequest, IRequest<AddUserResponse> { }

public sealed class AddUserCommandValidator : AbstractValidatorBase<AddUserCommand>
{
    public AddUserCommandValidator()
    {
        Include(new AddUserRequestValidator());
    }
}

public sealed class AddUserCommandHandler(IDatabaseService databaseService,
    IOtpService otpService,
    ICurrentUserService currentUserService,
    ILocalIdentityService localIdentityService,
    IEmailBlast2Service emailService,
    IOptions<AppConfigBackEndOptions> appConfigBackEndOptions)
    : IRequestHandler<AddUserCommand, AddUserResponse>
{
    private readonly AppConfigBackEndOptions _appConfigBackEndOptions = appConfigBackEndOptions.Value;
    public async Task<AddUserResponse> Handle(AddUserCommand request, CancellationToken cancellationToken)
    {
        var username = currentUserService.Username
            ?? throw ExceptionFor.NotAuthenticated();

        var checkDuplicate = await databaseService.Users
            .Where(x => !x.IsDeleted && x.Username.Trim() == request.EmailAddress)
            .AnyAsync(cancellationToken);

        if (checkDuplicate)
        {
            throw ExceptionFor.EntityAlreadyExists(UsersDisplayTextFor.Username, UsersDisplayTextFor.Username, username);
        }

        var configuration = await databaseService.Configurations
            .Where(x => !x.IsDeleted)
            .ToListAsync(cancellationToken);

        var defaultPass = configuration
            .SingleOrDefault(x => x.Key == KeyFor.DefaultPass)
            ?? throw ExceptionFor.EntityNotFound(ConfigurationsDisplayTextFor.Configuration, ConfigurationsDisplayTextFor.Key, KeyFor.DefaultPass);

        var companyName = configuration
            .SingleOrDefault(x => x.Key == KeyFor.CompanyName)
            ?? throw ExceptionFor.EntityNotFound(ConfigurationsDisplayTextFor.Configuration, ConfigurationsDisplayTextFor.Key, KeyFor.CompanyName);

        var userId = await localIdentityService.CreateUserAsync(request.EmailAddress, request.EmailAddress, defaultPass.Value);
        var otp = otpService.CreateOtp(request.EmailAddress);

        var user = new User
        {
            IdentityUserId = userId,
            LenderId = request.LenderId,
            Username = request.EmailAddress,
            DisplayName = request.Name,
            EmailAddress = request.EmailAddress,
            PhoneCode = request.CountryPhoneCode,
            PhoneNumber = request.PhoneNumber,
            OtpSecret = otp.Secret,
            OtpUrl = otp.Url,
            IsVerified = false,
            IsPicLender = false
        };

        _ = await databaseService.Users.AddAsync(user, cancellationToken);
        _ = await databaseService.SaveAsync(nameof(AddUser), cancellationToken);

        var randomToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
        var tokenPayload = $"{user.Id}.{randomToken}";

        var hash = Convert.ToHexString(
            SHA256.HashData(Encoding.UTF8.GetBytes(tokenPayload))
        );

        var expiredTokenTime = configuration
            .SingleOrDefault(x => x.Key == KeyFor.TokenExpiredHour)
            ?? throw ExceptionFor.EntityNotFound(ConfigurationsDisplayTextFor.Configuration, ConfigurationsDisplayTextFor.Key, KeyFor.TokenExpiredHour);

        var tokenExpired = TimeZoneInfo.ConvertTime(DateTimeOffset.Now.AddHours(Convert.ToInt32(expiredTokenTime.Value)), TimezoneFor.WibTimeZone);
        user.AccessTokenHash = hash;
        user.AccessTokenExpiredAt = tokenExpired;

        _ = await databaseService.SaveAsync(nameof(UpdateUser), cancellationToken);

        var encodedToken = Uri.EscapeDataString(randomToken);
        var defaultFrom = configuration
            .SingleOrDefault(x => x.Key == KeyFor.DefaultFrom)
            ?? throw ExceptionFor.EntityNotFound(ConfigurationsDisplayTextFor.Configuration, ConfigurationsDisplayTextFor.Key, KeyFor.DefaultFrom);

        var defaultAdminEmail = configuration
            .SingleOrDefault(x => x.Key == KeyFor.DefaultAdminEmail)
            ?? throw ExceptionFor.EntityNotFound(ConfigurationsDisplayTextFor.Configuration, ConfigurationsDisplayTextFor.Key, KeyFor.DefaultAdminEmail);

        var tos = new List<EmailContact2>()
        {
            new()
            {
                Address = user.EmailAddress,
                Name = user.DisplayName
            }
        };

        var newUser = await databaseService.Users
            .AsNoTracking()
            .Include(x => x.Lender)
            .Where(x => !x.IsDeleted && x.Id == user.Id)
            .SingleOrDefaultAsync(cancellationToken)
            ?? throw ExceptionFor.EntityNotFound(UsersDisplayTextFor.User, CommonDisplayTextFor.Id, user.Id);

        var parametersBodyEmail = new Dictionary<string, string>
        {
            ["DisplayName"] = newUser.DisplayName,
            ["Username"] = newUser.Username,
            ["LenderName"] = newUser.Lender.Name,
            ["RegistrationDate"] = $"{newUser.Created}",
            ["ExpiredDate"] = $"{newUser.AccessTokenExpiredAt}",
            ["VerificationLink"] = $"<a href=\"{_appConfigBackEndOptions.FrontEndBaseUrl}/UserVerification/{newUser.Id}?token={encodedToken}\">Here</a>",
            ["EmailGroup"] = $"{defaultAdminEmail.Value}",
            ["FrontendLink"] = $"<a href=\"{_appConfigBackEndOptions.FrontEndBaseUrl}\">{_appConfigBackEndOptions.AppNickName}</a> - {companyName.Value}",
        };

        var internalTos = tos
            .Where(x => x.Address.Contains("pertamina", StringComparison.OrdinalIgnoreCase))
            .ToList();

        var externalTos = tos
            .Where(x => !x.Address.Contains("pertamina", StringComparison.OrdinalIgnoreCase))
            .ToList();

        if (internalTos.Count == 0 && externalTos.Count == 0)
        {
            return new AddUserResponse
            {
                Item = new UserItem
                {
                    Id = user.Id,
                    Username = user.Username
                }
            };
        }

        if (internalTos.Count > 0)
        {
            var sendEmailInput = await EmailInputBuilder.BuildTheTemplate(databaseService, EmailTemplatesEmailWith.EmailBlast, CommonModuleFor.ExternalUsers,
                CommonActionFor.SendVerificationCode, parametersBodyEmail, defaultFrom: defaultFrom.Value, explicitTos: internalTos);

            emailService.SendEmails(sendEmailInput);
        }

        if (externalTos.Count > 0)
        {
            var sendEmailInput = await EmailInputBuilder.BuildTheTemplate(databaseService, EmailTemplatesEmailWith.TwilioSendGrid, CommonModuleFor.ExternalUsers,
                CommonActionFor.SendVerificationCode, parametersBodyEmail, defaultFrom: defaultFrom.Value, explicitTos: externalTos);

            emailService.SendEmails(sendEmailInput);
        }

        return new AddUserResponse
        {
            Item = new UserItem
            {
                Id = user.Id,
                Username = user.Username
            }
        };
    }
}
