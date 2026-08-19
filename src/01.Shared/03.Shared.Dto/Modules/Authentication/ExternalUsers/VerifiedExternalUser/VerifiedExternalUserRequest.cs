namespace EBVL.Shared.Dto.Modules.Authentication.ExternalUsers.VerifiedExternalUser;

public record VerifiedExternalUserRequest
{
    public required Guid ExternalLoginId { get; set; }

    public required string VerificationCode { get; set; }
}

public sealed class VerifiedExternalUserRequestValidator : AbstractValidatorBase<VerifiedExternalUserRequest>
{
    public VerifiedExternalUserRequestValidator()
    {
        _ = RuleFor(x => x.ExternalLoginId)
            .NotEmpty();

        _ = RuleFor(x => x.VerificationCode)
            .NotEmpty()
            .MinimumLength(UsersMinimumLengthFor.VerificationCode)
            .MaximumLength(UsersMaximumLengthFor.VerificationCode);
    }
}
