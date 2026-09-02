namespace EBVL.Shared.Dto.Modules.Authentication.ExternalUsers.LoginExternalUser;

public record LoginExternalUserRequest
{
    public required string Username { get; set; }
    public required string Password { get; set; }
}

public sealed class LoginExternalUserRequestValidator : AbstractValidatorBase<LoginExternalUserRequest>
{
    public LoginExternalUserRequestValidator()
    {
        _ = RuleFor(x => x.Username)
            .NotEmpty()
            .MaximumLength(CommonMaximumLengthFor.UserPrincipalName);

        _ = RuleFor(x => x.Password)
            .NotEmpty()
            .MaximumLength(UsersMaximumLengthFor.Password);
    }
}
