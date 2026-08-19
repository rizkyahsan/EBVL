using EBVL.BackEnd.Logics.Common.Builder;
using EBVL.BackEnd.Services.AppConfigBackEnd;
using EBVL.BackEnd.Services.EmailBlast2;
using EBVL.BackEnd.Services.EmailBlast2.Model;
using EBVL.BackEnd.Services.LocalIdentity;
using EBVL.Shared.Dto.Modules.MasterData.Users.SendOtpUser;
using EBVL.Shared.Statics.Configurations;
using Microsoft.Extensions.Options;
using Pertamina.Services.Otp;

namespace EBVL.BackEnd.Logics.Modules.MasterData.Users.SendOtpUser;

public sealed record SendOtpUserCommand : SendOtpUserRequest, IRequest<SendOtpUserResponse> { }

public sealed class SendOtpUserCommandValidator : AbstractValidatorBase<SendOtpUserCommand>
{
    public SendOtpUserCommandValidator()
    {
        Include(new SendOtpUserRequestValidator());
    }
}

public sealed class SendOtpUserCommandHandler(IDatabaseService databaseService,
    ILocalIdentityService localIdentityService,
    IEmailBlast2Service emailService,
    IOtpService otpService,
    IOptions<AppConfigBackEndOptions> appConfigBackEndOptions)
    : IRequestHandler<SendOtpUserCommand, SendOtpUserResponse>
{
    private readonly AppConfigBackEndOptions _appConfigBackEndOptions = appConfigBackEndOptions.Value;

    public async Task<SendOtpUserResponse> Handle(SendOtpUserCommand request, CancellationToken cancellationToken)
    {
        var isValid = await localIdentityService.CheckAccessAsync(request.UserId, request.Token);

        if (!isValid)
        {
            throw new InvalidOperationException($"{CommonDisplayTextFor.AccessDenied}!");
        }

        var user = await databaseService.Users
            .Where(x => !x.IsDeleted && x.Id == request.UserId)
            .SingleOrDefaultAsync(cancellationToken)
            ?? throw ExceptionFor.EntityNotFound(UsersDisplayTextFor.User, CommonDisplayTextFor.Id, request.UserId);

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
            return new SendOtpUserResponse
            {
                Item = new SendOtpUserResult
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

        return new SendOtpUserResponse
        {
            Item = new SendOtpUserResult
            {
                Message = $"The verification code has been successfully sent to {string.Join(", ", tos.Select(x => x.Address))}."
            }
        };
    }
}
