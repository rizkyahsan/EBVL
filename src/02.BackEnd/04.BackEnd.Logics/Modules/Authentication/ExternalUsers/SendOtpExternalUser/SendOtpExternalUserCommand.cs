using EBVL.BackEnd.Logics.Common.Builder;
using EBVL.BackEnd.Services.AppConfigBackEnd;
using EBVL.BackEnd.Services.EmailBlast2;
using EBVL.BackEnd.Services.EmailBlast2.Model;
using EBVL.Shared.Dto.Modules.Authentication.ExternalUsers.SendOtpExternalUser;
using EBVL.Shared.Statics.Configurations;
using Microsoft.Extensions.Options;
using Pertamina.Services.Otp;

namespace EBVL.BackEnd.Logics.Modules.Authentication.ExternalUsers.SendOtpExternalUser;

public sealed record SendOtpExternalUserCommand : SendOtpExternalUserRequest, IRequest<SendOtpExternalUserResponse> { }

public sealed class SendOtpExternalUserCommandValidator : AbstractValidatorBase<SendOtpExternalUserCommand>
{
    public SendOtpExternalUserCommandValidator()
    {
        Include(new SendOtpExternalUserRequestValidator());
    }
}

public sealed class SendOtpExternalUserCommandHandler(IDatabaseService databaseService,
    IEmailBlast2Service emailService,
    IOtpService otpService,
    IOptions<AppConfigBackEndOptions> appConfigBackEndOptions)
    : IRequestHandler<SendOtpExternalUserCommand, SendOtpExternalUserResponse>
{
    private readonly AppConfigBackEndOptions _appConfigBackEndOptions = appConfigBackEndOptions.Value;

    public async Task<SendOtpExternalUserResponse> Handle(SendOtpExternalUserCommand request, CancellationToken cancellationToken)
    {
        var externalLogin = await databaseService.ExternalLogins
            .Where(x => !x.IsDeleted && x.Id == request.ExternalLoginId)
            .SingleOrDefaultAsync(cancellationToken)
            ?? throw new InvalidOperationException($"{CommonDisplayTextFor.AccessDenied}!");

        var user = await databaseService.Users
            .Where(x => !x.IsDeleted && x.Id == externalLogin.UserId)
            .SingleOrDefaultAsync(cancellationToken)
            ?? throw new InvalidOperationException($"{CommonDisplayTextFor.AccessDenied}!");

        var verificationCode = otpService.GetCode(user.OtpSecret!);

        var defaultFrom = await databaseService.Configurations
           .Where(x => !x.IsDeleted && x.Key == KeyFor.DefaultFrom)
           .SingleOrDefaultAsync(cancellationToken)
           ?? throw new InvalidOperationException($"{CommonDisplayTextFor.AccessDenied}!");

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
            ["VerificationCode"] = verificationCode,
            ["AppName"] = _appConfigBackEndOptions.AppNickName,
            ["FrontendLink"] = $"<a href=\"{_appConfigBackEndOptions.FrontEndBaseUrl}\">{_appConfigBackEndOptions.AppNickName}</a>",
        };

        var internalTos = tos
            .Where(x => x.Address.Contains("pertamina", StringComparison.OrdinalIgnoreCase))
            .ToList();

        var externalTos = tos
            .Where(x => !x.Address.Contains("pertamina", StringComparison.OrdinalIgnoreCase))
            .ToList();

        if (internalTos.Count == 0 && externalTos.Count == 0)
        {
            return new SendOtpExternalUserResponse
            {
                Item = new SendOtpExternalUserResult
                {
                    Message = $"No Email Sended"
                }
            };
        }

        if (internalTos.Count > 0)
        {
            var sendEmailInput = await EmailInputBuilder.BuildTheTemplate(databaseService, EmailTemplatesEmailWith.EmailBlast, CommonModuleFor.ExternalUsers,
                CommonActionFor.SendOtp, parametersBodyEmail, defaultFrom: defaultFrom.Value, explicitTos: internalTos);

            emailService.SendEmails(sendEmailInput);
        }

        if (externalTos.Count > 0)
        {
            var sendEmailInput = await EmailInputBuilder.BuildTheTemplate(databaseService, EmailTemplatesEmailWith.TwilioSendGrid, CommonModuleFor.ExternalUsers,
                CommonActionFor.SendOtp, parametersBodyEmail, defaultFrom: defaultFrom.Value, explicitTos: externalTos);

            emailService.SendEmails(sendEmailInput);
        }

        return new SendOtpExternalUserResponse
        {
            Item = new SendOtpExternalUserResult
            {
                Message = $"The verification code has been successfully sent to {string.Join(", ", tos.Select(x => x.Address))}."
            }
        };
    }
}
