namespace EBVL.Shared.Dto.Modules.MasterData.Lenders.AddLender;

public record AddLenderRequest
{
    public required string Name { get; set; }
    public required string Address { get; set; }
    public required Guid CountryId { get; set; }
    public required string PhoneNumber { get; set; }
    public required string EmailAddress { get; set; }
    public required string Website { get; set; }
}

public sealed class AddLenderRequestValidator : AbstractValidatorBase<AddLenderRequest>
{
    public AddLenderRequestValidator()
    {
        _ = RuleFor(x => x.Name)
            .NotEmpty()
            .MinimumLength(LendersMinimumLengthFor.Name)
            .MaximumLength(LendersMaximumLengthFor.Name);

        _ = RuleFor(x => x.Address)
            .NotEmpty();

        _ = RuleFor(x => x.CountryId)
            .NotEmpty();

        _ = RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .MaximumLength(CommonMaximumLengthFor.PhoneNumber);

        _ = RuleFor(x => x.EmailAddress)
            .NotEmpty()
            .MinimumLength(CommonMinimumLengthFor.EmailAddress)
            .MaximumLength(CommonMaximumLengthFor.EmailAddress);

        _ = RuleFor(x => x.Website)
            .NotEmpty()
            .MinimumLength(CommonMinimumLengthFor.Url)
            .MaximumLength(CommonMaximumLengthFor.Url);
    }
}
