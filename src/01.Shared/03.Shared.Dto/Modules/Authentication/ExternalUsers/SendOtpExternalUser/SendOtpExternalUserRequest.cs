namespace EBVL.Shared.Dto.Modules.Authentication.ExternalUsers.SendOtpExternalUser;

public record SendOtpExternalUserRequest
{
    public required Guid ExternalLoginId { get; init; }
}

public sealed class SendOtpExternalUserRequestValidator : AbstractValidatorBase<SendOtpExternalUserRequest>
{
    public SendOtpExternalUserRequestValidator()
    {
        _ = RuleFor(x => x.ExternalLoginId)
            .NotEmpty();
    }
}
