namespace EBVL.Shared.Dto.Modules.MasterData.Users.AddUser;

public record AddUserRequest
{
    public required Guid LenderId { get; set; }
    public required string Name { get; set; }
    public required string EmailAddress { get; set; }
    public required string? CountryPhoneCode { get; set; }
    public required string? PhoneNumber { get; set; }
}

public sealed class AddUserRequestValidator : AbstractValidatorBase<AddUserRequest>
{
    public AddUserRequestValidator()
    {
        _ = RuleFor(x => x.LenderId)
            .NotEmpty();

        _ = RuleFor(x => x.Name)
            .NotEmpty()
            .MinimumLength(CommonMinimumLengthFor.PersonName)
            .MaximumLength(CommonMaximumLengthFor.PersonName);

        _ = RuleFor(x => x.EmailAddress)
            .NotEmpty()
            .EmailAddress()
            .MinimumLength(CommonMinimumLengthFor.EmailAddress)
            .MaximumLength(CommonMaximumLengthFor.EmailAddress);

        _ = When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber), () =>
        {
            _ = RuleFor(x => x.CountryPhoneCode)
                .NotEmpty();

            _ = RuleFor(x => x.PhoneNumber)
                .NotEmpty()
                .MaximumLength(CommonMaximumLengthFor.PhoneNumber);
        });
    }
}
