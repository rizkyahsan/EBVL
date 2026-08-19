namespace EBVL.Shared.Dto.Modules.Main.Users.UpdateMyUser;

public record UpdateMyUserRequest
{
    public required string Name { get; set; }
    public required string EmailAddress { get; set; }
    public required string? PhoneNumber { get; set; }
    public required string VerificationCode { get; set; }
}

public sealed class UpdateMyUserRequestValidator : AbstractValidator<UpdateMyUserRequest>
{
    public UpdateMyUserRequestValidator()
    {
        _ = RuleFor(x => x.Name)
            .NotEmpty()
            .MinimumLength(CommonMinimumLengthFor.PersonName)
            .MaximumLength(CommonMaximumLengthFor.PersonName);

        _ = RuleFor(x => x.EmailAddress)
            .NotEmpty()
            .MinimumLength(CommonMinimumLengthFor.EmailAddress)
            .MaximumLength(CommonMaximumLengthFor.EmailAddress);

        _ = When(request => !string.IsNullOrWhiteSpace(request.PhoneNumber), () =>
        {
            _ = RuleFor(x => x.PhoneNumber)
                .MinimumLength(CommonMinimumLengthFor.PhoneNumber)
                .MaximumLength(CommonMaximumLengthFor.PhoneNumber);
        });

        _ = RuleFor(x => x.VerificationCode)
            .NotEmpty()
            .MinimumLength(UsersMinimumLengthFor.VerificationCode)
            .MaximumLength(UsersMaximumLengthFor.VerificationCode);
    }
}
