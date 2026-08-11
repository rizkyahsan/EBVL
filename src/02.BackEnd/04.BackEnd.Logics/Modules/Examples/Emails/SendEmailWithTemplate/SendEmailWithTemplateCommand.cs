using System.Text;
using Microsoft.Extensions.Options;
using Pertamina.Services.CurrentUser;
using Pertamina.Services.Email;
using Pertamina.Services.UserProfile;
using EBVL.BackEnd.Services.AppConfigBackEnd;
using EBVL.Shared.Dto.Modules.Examples.Emails.SendEmailWithTemplate;

namespace EBVL.BackEnd.Logics.Modules.Examples.Emails.SendEmailWithTemplate;

[AuthorizeRequest]
public sealed record SendEmailWithTemplateCommand : SendEmailWithTemplateRequest, IRequest<SendEmailWithTemplateResponse>
{
}

public sealed class SendEmailWithTemplateCommandValidator : AbstractValidatorBase<SendEmailWithTemplateCommand>
{
    public SendEmailWithTemplateCommandValidator()
    {
        Include(new SendEmailWithTemplateRequestValidator());
    }
}

public sealed class SendEmailWithTemplateCommandHandler(
    IEmailService emailService,
    ICurrentUserService currentUserService,
    IUserProfileService userProfileService,
    IOptions<AppConfigBackEndOptions> appConfigBackEndOptions)
    : IRequestHandler<SendEmailWithTemplateCommand, SendEmailWithTemplateResponse>
{
    private static readonly string _templatePath = Path.Combine("EmailTemplates", "Template01.html");
    private readonly AppConfigBackEndOptions _appConfigBackEndOptions = appConfigBackEndOptions.Value;

    public async Task<SendEmailWithTemplateResponse> Handle(SendEmailWithTemplateCommand request, CancellationToken cancellationToken)
    {
        var tos = request.Tos.Select(x => new EmailContact
        {
            Address = x.Address,
            Name = x.Name
        }).ToList();

        var username = currentUserService.Username;
        var recipientName = "Valued Customer";

        if (!string.IsNullOrWhiteSpace(username))
        {
            var userProfile = await userProfileService.GetUserProfileAsync(username, cancellationToken);
            recipientName = userProfile.DisplayName;

            tos.Add(new EmailContact
            {
                Address = userProfile.EmailAddress,
                Name = userProfile.DisplayName
            });
        }

        var ccs = request.Ccs.Select(x => new EmailContact
        {
            Address = x.Address,
            Name = x.Name
        }).ToList();

        var bccs = request.Bccs.Select(x => new EmailContact
        {
            Address = x.Address,
            Name = x.Name
        }).ToList();

        var emailTemplateHtmlContent = await File.ReadAllTextAsync(_templatePath, cancellationToken);

        var random = new Random();
        var orderNumber = random.Next(1001, 10_000);

        var subject = $"[{_appConfigBackEndOptions.AppNickName}] Your order #{orderNumber} receipt ({request.ItemsCount} items)";

        var orderDetailsBuilder = new StringBuilder();
        _ = orderDetailsBuilder.AppendLine("<table><thead><tr>");
        _ = orderDetailsBuilder.AppendLine("<th>No</th>");
        _ = orderDetailsBuilder.AppendLine("<th>Product Name</th>");
        _ = orderDetailsBuilder.AppendLine("<th>Unit Price</th>");
        _ = orderDetailsBuilder.AppendLine("<th>Quantity</th>");
        _ = orderDetailsBuilder.AppendLine("<th>Sub Total</th>");
        _ = orderDetailsBuilder.AppendLine("</tr></thead><tbody>");

        var total = 0m;

        for (var i = 1; i <= request.ItemsCount; i++)
        {
            var unitPrice = random.Next(10, 91) * 1000m;
            var quantity = random.Next(1, 11);
            var subtotal = unitPrice * quantity;
            total += subtotal;

            _ = orderDetailsBuilder.AppendLine("<tr>");
            _ = orderDetailsBuilder.AppendLine($"<td style=\"text-align: center;\">{i}</td>");
            _ = orderDetailsBuilder.AppendLine($"<td>Product {i}</td>");
            _ = orderDetailsBuilder.AppendLine($"<td style=\"text-align: right;\">{unitPrice.ToDisplayText(CurrencyFormatFor.NoDecimal)}</td>");
            _ = orderDetailsBuilder.AppendLine($"<td style=\"text-align: center;\">{quantity}</td>");
            _ = orderDetailsBuilder.AppendLine($"<td style=\"text-align: right;\">{subtotal.ToDisplayText(CurrencyFormatFor.NoDecimal)}</td>");
            _ = orderDetailsBuilder.AppendLine("</tr>");
        }

        _ = orderDetailsBuilder.AppendLine("<tr>");
        _ = orderDetailsBuilder.AppendLine($"<td colspan=\"4\" style=\"text-align: right; font-weight: bold;\">TOTAL</td>");
        _ = orderDetailsBuilder.AppendLine($"<td style=\"text-align: right; font-weight: bold;\">{total.ToDisplayText(CurrencyFormatFor.NoDecimal)}</td>");
        _ = orderDetailsBuilder.AppendLine("</tr>");

        _ = orderDetailsBuilder.AppendLine("</tbody>");
        _ = orderDetailsBuilder.AppendLine("</table>");

        var replacements = new Dictionary<string, string>
        {
            { "{{RecipientName}}", recipientName },
            { "{{OrderDetails}}", orderDetailsBuilder.ToString() },
            { "{{AppNickName}}", _appConfigBackEndOptions.AppNickName },
        };

        var body = emailTemplateHtmlContent.Replace(replacements);

        var attachments = request.Attachments.Select(x => new EmailAttachment
        {
            FileName = x.FileName,
            FileContent = x.FileContent
        }).ToList();

        var sendEmailInput = new SendEmailInput
        {
            Tos = tos,
            Ccs = ccs,
            Bccs = bccs,
            Subject = subject,
            Body = body.ToString(),
            Attachments = attachments
        };

        emailService.SendEmail(sendEmailInput);

        var recipientsCount = tos.Count + ccs.Count + bccs.Count;

        return new SendEmailWithTemplateResponse
        {
            Item = new SendEmailWithTemplateResult
            {
                Message = $"Email with subject {sendEmailInput.Subject} and {attachments.Count} attachment(s) has been successfully sent to the {recipientsCount} recipient(s)."
            }
        };
    }
}
