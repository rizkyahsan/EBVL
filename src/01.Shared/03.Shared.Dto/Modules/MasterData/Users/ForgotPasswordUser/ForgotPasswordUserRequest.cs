namespace EBVL.Shared.Dto.Modules.MasterData.Users.ForgotPasswordUser;

public record ForgotPasswordUserRequest
{
    public required string UsernameOrEmail { get; set; }
}

public sealed class ForgotPasswordUserRequestValidator : AbstractValidatorBase<ForgotPasswordUserRequest>
{
    public ForgotPasswordUserRequestValidator()
    {
        _ = RuleFor(x => x.UsernameOrEmail)
            .NotEmpty()
            .MaximumLength(256);
    }
}
