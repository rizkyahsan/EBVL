using System.Text;
using Microsoft.Extensions.Options;
using Pertamina.Services.CurrentUser;
using Pertamina.Services.Email;
using Pertamina.Services.Email.Statics;
using Pertamina.Services.Otp;
using EBVL.BackEnd.Services.AppConfigBackEnd;
using EBVL.Shared.Dto.Modules.Main.Users.SendMyVerificationCode;

namespace EBVL.BackEnd.Logics.Modules.Main.Users.SendMyVerificationCode;

public sealed record SendMyVerificationCodeCommand : IRequest<SendMyVerificationCodeResponse>
{
}

public sealed class SendMyVerificationCodeCommandHandler(
    ICurrentUserService currentUserService,
    IDatabaseService databaseService,
    IEmailService emailService,
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

        var verificationCode = otpService.GetCode(user.OtpSecret);

        var body = new StringBuilder();
        _ = body.AppendLine($"<p>Dear {user.Name},</p>");
        _ = body.AppendLine("<p>Your verification code is:</p>");
        _ = body.AppendLine($"<div style=\"margin: 12px 0px; padding: 12px; border: solid 1px black; font-size: 36px; font-weight: bold;\">{verificationCode}</div>");
        _ = body.AppendLine("<p>If you did not request this code, please ignore this email.</p>");
        _ = body.AppendLine("<br />");
        _ = body.AppendLine($"<p>Best regards,<br />The {_appConfigBackEndOptions.AppNickName} Team</p>");
        _ = body.AppendLine($"<p>Visit <a href=\"{ContentPlaceholderFor.FrontEndUrl}/\">{_appConfigBackEndOptions.AppNickName}</a>.</p>");

        var to = new EmailContact
        {
            Address = user.EmailAddress,
            Name = user.Name
        };

        var sendEmailInput = new SendEmailInput
        {
            Tos = new[] { to },
            Subject = "Your Verification Code",
            Body = body.ToString()
        };

        emailService.SendEmail(sendEmailInput);

        return new SendMyVerificationCodeResponse
        {
            Item = new SendMyVerificationCodeResult
            {
                Message = $"The verification code has been successfully sent to {to.Address}."
            }
        };
    }
}
