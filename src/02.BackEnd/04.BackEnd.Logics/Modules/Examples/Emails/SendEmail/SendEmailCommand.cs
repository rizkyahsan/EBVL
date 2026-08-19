using System.Text;
using EBVL.BackEnd.Services.AppConfigBackEnd;
using EBVL.BackEnd.Services.EmailBlast2;
using EBVL.BackEnd.Services.EmailBlast2.Model;
using EBVL.Shared.Dto.Modules.Examples.Emails.SendEmail;
using Microsoft.Extensions.Options;
using Pertamina.Services.CurrentUser;
using Pertamina.Services.Email.Statics;
using Pertamina.Services.UserProfile;

namespace EBVL.BackEnd.Logics.Modules.Examples.Emails.SendEmail;

[AuthorizeRequest]
public sealed record SendEmailCommand : SendEmailRequest, IRequest<SendEmailResponse>
{
}

public sealed class SendEmailCommandValidator : AbstractValidatorBase<SendEmailCommand>
{
    public SendEmailCommandValidator()
    {
        Include(new SendEmailRequestValidator());
    }
}

public sealed class SendEmailCommandHandler(
    IEmailBlast2Service emailService,
    ICurrentUserService currentUserService,
    IUserProfileService userProfileService,
    IOptions<AppConfigBackEndOptions> appConfigBackEndOptions)
    : IRequestHandler<SendEmailCommand, SendEmailResponse>
{
    private readonly AppConfigBackEndOptions _appConfigBackEndOptions = appConfigBackEndOptions.Value;

    public async Task<SendEmailResponse> Handle(SendEmailCommand request, CancellationToken cancellationToken)
    {
        var tos = request.Tos.Select(x => new EmailContact2
        {
            Address = x.Address,
            Name = x.Name
        }).ToList();

        var username = currentUserService.Username;

        if (!string.IsNullOrWhiteSpace(username))
        {
            var userProfile = await userProfileService.GetUserProfileAsync(username, cancellationToken);

            tos.Add(new EmailContact2
            {
                Address = userProfile.EmailAddress,
                Name = userProfile.DisplayName
            });
        }

        var ccs = request.Ccs.Select(x => new EmailContact2
        {
            Address = x.Address,
            Name = x.Name
        }).ToList();

        var bccs = request.Bccs.Select(x => new EmailContact2
        {
            Address = x.Address,
            Name = x.Name
        }).ToList();

        var body = new StringBuilder(request.Body);
        _ = body.AppendLine($"<p>Visit <a href=\"{ContentPlaceholderFor.FrontEndUrl}/\">{_appConfigBackEndOptions.AppNickName}</a>.</p>");

        var sendEmailInput = new SendEmailInput2
        {
            Module = string.Empty,
            Action = string.Empty,
            EmailWith = EmailTemplatesEmailWith.TwilioSendGrid,
            Tos = tos,
            Ccs = ccs,
            Bccs = bccs,
            Subject = request.Subject,
            Body = body.ToString()
        };

        emailService.SendEmails(sendEmailInput);

        var recipientsCount = tos.Count + ccs.Count + bccs.Count;

        return new SendEmailResponse
        {
            Item = new SendEmailResult
            {
                Message = $"Email with subject {sendEmailInput.Subject} attachment(s) has been successfully sent to the {recipientsCount} recipient(s)."
            }
        };
    }
}
