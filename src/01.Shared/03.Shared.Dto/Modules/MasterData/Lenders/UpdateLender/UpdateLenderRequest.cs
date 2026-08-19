namespace EBVL.Shared.Dto.Modules.MasterData.Lenders.UpdateLender;

public record UpdateLenderRequest
{
    public required Guid LenderId { get; init; }
    public required string Name { get; set; }
    public required string Address { get; set; }
    public required Guid CountryId { get; set; }
    public required string PhoneNumber { get; set; }
    public required string EmailAddress { get; set; }
    public required string Website { get; set; }
}

public sealed class UpdateLenderRequestValidator : AbstractValidatorBase<UpdateLenderRequest>
{
    public UpdateLenderRequestValidator()
    {
        _ = RuleFor(x => x.LenderId)
            .NotEmpty();

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
