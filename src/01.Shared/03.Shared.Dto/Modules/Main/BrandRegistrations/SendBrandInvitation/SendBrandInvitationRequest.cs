namespace EBVL.Shared.Dto.Modules.Main.BrandRegistrations.SendBrandInvitation;

public sealed record SendBrandInvitationRequest(string Subject, string Recipient, string Cc, string Body, string RowVersion);

public sealed class SendBrandInvitationRequestValidator : AbstractValidator<SendBrandInvitationRequest>
{
    public SendBrandInvitationRequestValidator()
    {
        _ = RuleFor(x => x.Subject).NotEmpty().MaximumLength(300);
        _ = RuleFor(x => x.Recipient).NotEmpty().MaximumLength(1000).EmailAddress();
        _ = RuleFor(x => x.Cc).NotEmpty().MaximumLength(1000).EmailAddress();
        _ = RuleFor(x => x.Body).NotEmpty();
        _ = RuleFor(x => x.RowVersion).NotEmpty();
    }
}
