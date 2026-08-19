namespace EBVL.Shared.Dto.Modules.MasterData.Users.ResetPasswordUser;

public record ResetPasswordUserRequest
{
    public required Guid UserId { get; set; }

    public required string Token { get; init; }

    public required string VerificationCode { get; set; }

    public required string Password { get; set; }
}

public sealed class ResetPasswordUserRequestValidator : AbstractValidatorBase<ResetPasswordUserRequest>
{
    public ResetPasswordUserRequestValidator()
    {
        _ = RuleFor(x => x.UserId)
            .NotEmpty();

        _ = RuleFor(x => x.Token)
            .NotEmpty();

        _ = RuleFor(x => x.VerificationCode)
            .NotEmpty()
            .MinimumLength(UsersMinimumLengthFor.VerificationCode)
            .MaximumLength(UsersMaximumLengthFor.VerificationCode);

        _ = RuleFor(x => x.Password)
            .NotEmpty();
    }
}
