namespace EBVL.Shared.Dto.Modules.Authentication.ExternalUsers.CheckExternalUser;

public record CheckExternalUserRequest
{
    public required Guid ExternalLoginId { get; init; }
}

public sealed class CheckExternalUserRequestValidator : AbstractValidatorBase<CheckExternalUserRequest>
{
    public CheckExternalUserRequestValidator()
    {
        _ = RuleFor(x => x.ExternalLoginId)
            .NotEmpty();
    }
}
