namespace EBVL.Shared.Dto.Modules.MasterData.Users.UpdateUser;

public record UpdateUserRequest
{
    public required Guid UserId { get; set; }
    public required Guid LenderId { get; set; }
    public required string Name { get; set; }
    public required string LenderName { get; set; }
    public required string EmailAddress { get; set; }
    public required string? CountryPhoneCode { get; set; }
    public required string? PhoneNumber { get; set; }
}

public sealed class UpdateUserRequestValidator : AbstractValidatorBase<UpdateUserRequest>
{
    public UpdateUserRequestValidator()
    {
        _ = RuleFor(x => x.UserId)
            .NotEmpty();

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

        _ = When(request => !string.IsNullOrWhiteSpace(request.PhoneNumber), () =>
        {
            _ = RuleFor(x => x.CountryPhoneCode)
                .NotEmpty();

            _ = RuleFor(x => x.PhoneNumber)
                .NotEmpty()
                .MaximumLength(CommonMaximumLengthFor.PhoneNumber);
        });
    }
}
