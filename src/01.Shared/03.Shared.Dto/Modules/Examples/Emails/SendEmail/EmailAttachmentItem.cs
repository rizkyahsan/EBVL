namespace EBVL.Shared.Dto.Modules.Examples.Emails.SendEmail;

public sealed record EmailAttachmentItem : SendFileRequest
{
}

public sealed class EmailAttachmentItemValidator : AbstractValidatorBase<EmailAttachmentItem>
{
    public EmailAttachmentItemValidator()
    {
        Include(new SendFileRequestValidator());
    }
}
