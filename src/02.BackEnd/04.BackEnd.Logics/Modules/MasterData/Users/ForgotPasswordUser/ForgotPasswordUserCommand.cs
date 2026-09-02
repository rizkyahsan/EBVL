using System.Security.Cryptography;
using System.Text;
using EBVL.BackEnd.Logics.Common.Builder;
using EBVL.BackEnd.Services.AppConfigBackEnd;
using EBVL.BackEnd.Services.EmailBlast2;
using EBVL.BackEnd.Services.EmailBlast2.Model;
using EBVL.BackEnd.Services.LocalIdentity;
using EBVL.Shared.Dto.Modules.MasterData.Users.ForgotPasswordUser;
using EBVL.Shared.Statics.Common;
using EBVL.Shared.Statics.Configurations;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;

namespace EBVL.BackEnd.Logics.Modules.MasterData.Users.ForgotPasswordUser;

public sealed record ForgotPasswordUserCommand : ForgotPasswordUserRequest, IRequest<ForgotPasswordUserResponse>;

public sealed class ForgotPasswordUserCommandValidator : AbstractValidatorBase<ForgotPasswordUserCommand>
{
    public ForgotPasswordUserCommandValidator()
    {
        Include(new ForgotPasswordUserRequestValidator());
    }
}

public sealed class ForgotPasswordUserCommandHandler(
    IDatabaseService databaseService,
    IEmailBlast2Service emailService,
    ILocalIdentityService localIdentityService,
    IOptions<AppConfigBackEndOptions> appConfigBackEndOptions,
    ILogger<ForgotPasswordUserCommandHandler> logger)
    : IRequestHandler<ForgotPasswordUserCommand, ForgotPasswordUserResponse>
{
    private const string GenericMessage = "If an eligible account matches that username or email, a password reset link has been sent.";
    private readonly AppConfigBackEndOptions _options = appConfigBackEndOptions.Value;

    public async Task<ForgotPasswordUserResponse> Handle(ForgotPasswordUserCommand request, CancellationToken cancellationToken)
    {
        var identifier = request.UsernameOrEmail.Trim();
        try
        {
            var user = await databaseService.Users
                .Include(x => x.Lender)
                .Where(x => !x.IsDeleted && x.IsVerified && !x.Lender.IsDeleted &&
                    (x.Username == identifier || x.EmailAddress == identifier))
                .SingleOrDefaultAsync(cancellationToken);

            if (user is null || !await localIdentityService.IsUserEligibleAsync(user.Username))
            {
                return GenericResponse();
            }

            var configuration = await databaseService.Configurations
                .Where(x => !x.IsDeleted)
                .ToListAsync(cancellationToken);
            var companyName = configuration.Single(x => x.Key == KeyFor.CompanyName);
            var expiredTokenTime = configuration.Single(x => x.Key == KeyFor.TokenExpiredHour);
            var defaultFrom = configuration.Single(x => x.Key == KeyFor.DefaultFrom);
            var defaultAdminEmail = configuration.Single(x => x.Key == KeyFor.DefaultAdminEmail);

            var randomToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
            user.AccessTokenHash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes($"{user.Id}.{randomToken}")));
            var now = TimeZoneInfo.ConvertTime(DateTimeOffset.Now, TimezoneFor.WibTimeZone);
            user.AccessTokenExpiredAt = now.AddHours(Convert.ToInt32(expiredTokenTime.Value));
            _ = await databaseService.SaveAsync(nameof(ForgotPasswordUser), cancellationToken);

            var tos = new List<EmailContact2> { new() { Address = user.EmailAddress, Name = user.DisplayName } };
            var parameters = new Dictionary<string, string>
            {
                ["DisplayName"] = user.DisplayName,
                ["Username"] = user.Username,
                ["LenderName"] = user.Lender.Name,
                ["RequestDate"] = $"{now}",
                ["ExpiredDate"] = $"{user.AccessTokenExpiredAt}",
                ["VerificationLink"] = $"<a href=\"{_options.FrontEndBaseUrl}/ResetPassword/{user.Id}?token={Uri.EscapeDataString(randomToken)}\">Here</a>",
                ["EmailGroup"] = defaultAdminEmail.Value,
                ["FrontendLink"] = $"<a href=\"{_options.FrontEndBaseUrl}\">{_options.AppNickName}</a> - {companyName.Value}",
            };

            var internalTos = tos.Where(x => x.Address.Contains("pertamina", StringComparison.OrdinalIgnoreCase)).ToList();
            var externalTos = tos.Where(x => !x.Address.Contains("pertamina", StringComparison.OrdinalIgnoreCase)).ToList();
            if (internalTos.Count > 0)
            {
                var input = await EmailInputBuilder.BuildTheTemplate(databaseService, EmailTemplatesEmailWith.EmailBlast,
                    CommonModuleFor.ExternalUsers, CommonActionFor.SendResetPassword, parameters,
                    defaultFrom: defaultFrom.Value, explicitTos: internalTos);
                emailService.SendEmails(input);
            }

            if (externalTos.Count > 0)
            {
                var input = await EmailInputBuilder.BuildTheTemplate(databaseService, EmailTemplatesEmailWith.TwilioSendGrid,
                    CommonModuleFor.ExternalUsers, CommonActionFor.SendResetPassword, parameters,
                    defaultFrom: defaultFrom.Value, explicitTos: externalTos);
                emailService.SendEmails(input);
            }
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            logger.LogError(exception, "Unable to process a public password reset request.");
        }

        return GenericResponse();
    }

    private static ForgotPasswordUserResponse GenericResponse()
    {
        return new ForgotPasswordUserResponse
        {
            Item = new ForgotPasswordUserResult { Message = GenericMessage }
        };
    }
}
