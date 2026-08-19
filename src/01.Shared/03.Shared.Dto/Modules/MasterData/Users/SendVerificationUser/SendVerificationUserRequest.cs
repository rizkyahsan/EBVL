namespace EBVL.Shared.Dto.Modules.MasterData.Users.SendVerificationUser;

public record SendVerificationUserRequest
{
    public required Guid UserId { get; set; }
}

public sealed class SendVerificationUserRequestValidator : AbstractValidatorBase<SendVerificationUserRequest>
{
    public SendVerificationUserRequestValidator()
    {
        _ = RuleFor(x => x.UserId)
            .NotEmpty();
    }
}
