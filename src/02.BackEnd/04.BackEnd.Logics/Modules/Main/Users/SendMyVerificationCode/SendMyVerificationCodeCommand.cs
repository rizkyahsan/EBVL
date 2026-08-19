using EBVL.BackEnd.Logics.Common.Builder;
using EBVL.BackEnd.Services.AppConfigBackEnd;
using EBVL.BackEnd.Services.EmailBlast2;
using EBVL.BackEnd.Services.EmailBlast2.Model;
using EBVL.Shared.Dto.Modules.Main.Users.SendMyVerificationCode;
using EBVL.Shared.Statics.Configurations;
using Microsoft.Extensions.Options;
using Pertamina.Services.CurrentUser;
using Pertamina.Services.Otp;

namespace EBVL.BackEnd.Logics.Modules.Main.Users.SendMyVerificationCode;

[AuthorizeRequest]
public sealed record SendMyVerificationCodeCommand : IRequest<SendMyVerificationCodeResponse> { }

public sealed class SendMyVerificationCodeCommandHandler(
    ICurrentUserService currentUserService,
    IDatabaseService databaseService,
    IEmailBlast2Service emailService,
    IOtpService otpService,
    IOptions<AppConfigBackEndOptions> appConfigBackEndOptions)
    : IRequestHandler<SendMyVerificationCodeCommand, SendMyVerificationCodeResponse>
{
    private readonly AppConfigBackEndOptions _appConfigBackEndOptions = appConfigBackEndOptions.Value;

    public async Task<SendMyVerificationCodeResponse> Handle(SendMyVerificationCodeCommand request, CancellationToken cancellationToken)
    {
        var username = currentUserService.Username
            ?? throw ExceptionFor.NotAuthenticated();

        var user = await databaseService.Users
            .Where(x => !x.IsDeleted && x.Username == username)
            .SingleOrDefaultAsync(cancellationToken)
            ?? throw ExceptionFor.EntityNotFound(UsersDisplayTextFor.User, CommonDisplayTextFor.Username, username);

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

        var sendEmailInput = await EmailInputBuilder.BuildTheTemplate(databaseService, EmailTemplatesEmailWith.TwilioSendGrid, CommonModuleFor.ExternalUsers,
            CommonActionFor.SendOtp, parametersBodyEmail, defaultFrom: defaultFrom.Value, explicitTos: tos);

        emailService.SendEmails(sendEmailInput);

        return new SendMyVerificationCodeResponse
        {
            Item = new SendMyVerificationCodeResult
            {
                Message = $"The verification code has been successfully sent to {string.Join(", ", tos.Select(x => x.Address))}."
            }
        };
    }
}
