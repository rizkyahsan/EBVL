namespace EBVL.Shared.Dto.Modules.Main.Users.ReloadMyUser;

public record ReloadMyUserRequest
{
    public required string VerificationCode { get; set; }
}

public sealed class ReloadMyUserRequestValidator : AbstractValidator<ReloadMyUserRequest>
{
    public ReloadMyUserRequestValidator()
    {
        _ = RuleFor(x => x.VerificationCode)
            .NotEmpty()
            .MinimumLength(UsersMinimumLengthFor.VerificationCode)
            .MaximumLength(UsersMaximumLengthFor.VerificationCode);
    }
}
