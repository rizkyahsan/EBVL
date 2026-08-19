using System.Security.Cryptography;
using System.Text;
using EBVL.BackEnd.Logics.Common.Builder;
using EBVL.BackEnd.Services.AppConfigBackEnd;
using EBVL.BackEnd.Services.EmailBlast2;
using EBVL.BackEnd.Services.EmailBlast2.Model;
using EBVL.Shared.Dto.Modules.MasterData.Users.SendResetPasswordUser;
using EBVL.Shared.Statics.Common;
using EBVL.Shared.Statics.Configurations;
using Microsoft.Extensions.Options;
using Pertamina.Services.CurrentUser;

namespace EBVL.BackEnd.Logics.Modules.MasterData.Users.SendResetPasswordUser;

[AuthorizeRequest]
public sealed record SendResetPasswordUserCommand : SendResetPasswordUserRequest, IRequest<SendResetPasswordUserResponse> { }

public sealed class SendResetPasswordUserCommandValidator : AbstractValidatorBase<SendResetPasswordUserCommand>
{
    public SendResetPasswordUserCommandValidator()
    {
        Include(new SendResetPasswordUserRequestValidator());
    }
}

public sealed class SendResetPasswordUserCommandHandler(
    ICurrentUserService currentUserService,
    IDatabaseService databaseService,
    IEmailBlast2Service emailService,
    IOptions<AppConfigBackEndOptions> appConfigBackEndOptions)
    : IRequestHandler<SendResetPasswordUserCommand, SendResetPasswordUserResponse>
{
    private readonly AppConfigBackEndOptions _appConfigBackEndOptions = appConfigBackEndOptions.Value;

    public async Task<SendResetPasswordUserResponse> Handle(SendResetPasswordUserCommand request, CancellationToken cancellationToken)
    {
        var username = currentUserService.Username
            ?? throw ExceptionFor.NotAuthenticated();

        var user = await databaseService.Users
            .Include(x => x.Lender)
            .Where(x => !x.IsDeleted && x.Id == request.UserId)
            .SingleOrDefaultAsync(cancellationToken)
            ?? throw ExceptionFor.EntityNotFound(UsersDisplayTextFor.User, CommonDisplayTextFor.UserId, request.UserId);

        var configuration = await databaseService.Configurations
            .Where(x => !x.IsDeleted)
            .ToListAsync(cancellationToken);

        var companyName = configuration
            .SingleOrDefault(x => x.Key == KeyFor.CompanyName)
            ?? throw ExceptionFor.EntityNotFound(ConfigurationsDisplayTextFor.Configuration, ConfigurationsDisplayTextFor.Key, KeyFor.CompanyName);

        var randomToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
        var tokenPayload = $"{user.Id}.{randomToken}";

        var hash = Convert.ToHexString(
            SHA256.HashData(Encoding.UTF8.GetBytes(tokenPayload))
        );
        var expiredTokenTime = configuration
            .SingleOrDefault(x => x.Key == KeyFor.TokenExpiredHour)
            ?? throw ExceptionFor.EntityNotFound(ConfigurationsDisplayTextFor.Configuration, ConfigurationsDisplayTextFor.Key, KeyFor.TokenExpiredHour);

        var now = TimeZoneInfo.ConvertTime(DateTimeOffset.Now, TimezoneFor.WibTimeZone);
        var tokenExpired = now.AddHours(Convert.ToInt32(expiredTokenTime.Value));
        user.AccessTokenHash = hash;
        user.AccessTokenExpiredAt = tokenExpired;

        _ = await databaseService.SaveAsync(nameof(SendResetPasswordUser), cancellationToken);

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

        var parametersBodyEmail = new Dictionary<string, string>
        {
            ["DisplayName"] = user.DisplayName,
            ["Username"] = user.Username,
            ["LenderName"] = user.Lender.Name,
            ["RequestDate"] = $"{now}",
            ["ExpiredDate"] = $"{user.AccessTokenExpiredAt}",
            ["VerificationLink"] = $"<a href=\"{_appConfigBackEndOptions.FrontEndBaseUrl}/ResetPassword/{user.Id}?token={encodedToken}\">Here</a>",
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
            return new SendResetPasswordUserResponse
            {
                Item = new SendResetPasswordUserResult
                {
                    Message = $"No Email Sended"
                }
            };
        }

        if (internalTos.Count > 0)
        {
            var sendEmailInput = await EmailInputBuilder.BuildTheTemplate(databaseService, EmailTemplatesEmailWith.EmailBlast, CommonModuleFor.ExternalUsers,
                CommonActionFor.SendResetPassword, parametersBodyEmail, defaultFrom: defaultFrom.Value, explicitTos: internalTos);

            emailService.SendEmails(sendEmailInput);
        }

        if (externalTos.Count > 0)
        {
            var sendEmailInput = await EmailInputBuilder.BuildTheTemplate(databaseService, EmailTemplatesEmailWith.TwilioSendGrid, CommonModuleFor.ExternalUsers,
                CommonActionFor.SendResetPassword, parametersBodyEmail, defaultFrom: defaultFrom.Value, explicitTos: externalTos);

            emailService.SendEmails(sendEmailInput);
        }

        return new SendResetPasswordUserResponse
        {
            Item = new SendResetPasswordUserResult
            {
                Message = $"The reset password request has been successfully sent to {string.Join(", ", tos.Select(x => x.Address))}."
            }
        };
    }
}
