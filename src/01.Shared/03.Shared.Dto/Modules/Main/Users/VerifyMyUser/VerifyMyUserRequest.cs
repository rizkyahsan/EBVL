namespace EBVL.Shared.Dto.Modules.Main.Users.VerifyMyUser;

public record VerifyMyUserRequest
{
    public required string VerificationCode { get; set; }
}

public sealed class VerifyMyUserRequestValidator : AbstractValidator<VerifyMyUserRequest>
{
    public VerifyMyUserRequestValidator()
    {
        _ = RuleFor(x => x.VerificationCode)
            .NotEmpty()
            .MinimumLength(UsersMinimumLengthFor.VerificationCode)
            .MaximumLength(UsersMaximumLengthFor.VerificationCode);
    }
}
