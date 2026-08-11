namespace EBVL.Shared.Dto.Modules.Examples.Emails.SendEmailWithTemplate;

public record SendEmailWithTemplateRequest
{
    public required List<EmailContactItem> Tos { get; init; }
    public List<EmailContactItem> Ccs { get; init; } = [];
    public List<EmailContactItem> Bccs { get; init; } = [];
    public required int ItemsCount { get; init; }
    public List<EmailAttachmentItem> Attachments { get; init; } = [];
}

public sealed class SendEmailWithTemplateRequestValidator : AbstractValidatorBase<SendEmailWithTemplateRequest>
{
    public SendEmailWithTemplateRequestValidator()
    {
        _ = RuleFor(x => x.Tos)
            .NotEmpty();

        _ = RuleForEach(x => x.Tos)
            .SetValidator(new EmailContactItemValidator());

        _ = RuleForEach(x => x.Ccs)
            .SetValidator(new EmailContactItemValidator());

        _ = RuleForEach(x => x.Bccs)
            .SetValidator(new EmailContactItemValidator());

        _ = RuleFor(x => x.ItemsCount)
            .InclusiveBetween(2, 5);

        _ = RuleForEach(x => x.Attachments)
            .SetValidator(new EmailAttachmentItemValidator());
    }
}
