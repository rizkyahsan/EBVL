namespace EBVL.Shared.Dto.Modules.Examples.Emails.SendEmail;

public record SendEmailRequest
{
    public required List<EmailContactItem> Tos { get; init; }
    public List<EmailContactItem> Ccs { get; init; } = [];
    public List<EmailContactItem> Bccs { get; init; } = [];
    public required string Subject { get; init; }
    public required string Body { get; init; }
    public List<EmailAttachmentItem> Attachments { get; init; } = [];
}

public sealed class SendEmailRequestValidator : AbstractValidatorBase<SendEmailRequest>
{
    public SendEmailRequestValidator()
    {
        _ = RuleFor(x => x.Tos)
            .NotEmpty();

        _ = RuleForEach(x => x.Tos)
            .SetValidator(new EmailContactItemValidator());

        _ = RuleForEach(x => x.Ccs)
            .SetValidator(new EmailContactItemValidator());

        _ = RuleForEach(x => x.Bccs)
            .SetValidator(new EmailContactItemValidator());

        _ = RuleFor(x => x.Subject)
            .NotEmpty();

        _ = RuleFor(x => x.Body)
            .NotEmpty();

        _ = RuleForEach(x => x.Attachments)
            .SetValidator(new EmailAttachmentItemValidator());
    }
}
