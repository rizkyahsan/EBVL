namespace EBVL.Shared.Dto.Modules.MasterData.Users.VerifiedUser;

public record VerifiedUserRequest
{
    public required Guid UserId { get; set; }

    public required string Token { get; init; }

    public required string VerificationCode { get; set; }

    public required string Password { get; set; }
}

public sealed class VerifiedUserRequestValidator : AbstractValidatorBase<VerifiedUserRequest>
{
    public VerifiedUserRequestValidator()
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
