namespace EBVL.Shared.Dto.Modules.Examples.Emails.SendEmail;

public sealed record EmailContactItem
{
    public required string Name { get; init; }
    public required string Address { get; init; }
}

public sealed class EmailContactItemValidator : AbstractValidatorBase<EmailContactItem>
{
    public EmailContactItemValidator()
    {
        _ = RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(CommonMaximumLengthFor.PersonName);

        _ = RuleFor(x => x.Address)
            .NotEmpty()
            .EmailAddress()
            .Must(x => x.IsValidEmailAddress());
    }
}
