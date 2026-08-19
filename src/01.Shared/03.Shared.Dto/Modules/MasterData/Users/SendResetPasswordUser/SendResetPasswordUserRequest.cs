namespace EBVL.Shared.Dto.Modules.MasterData.Users.SendResetPasswordUser;

public record SendResetPasswordUserRequest
{
    public required Guid UserId { get; set; }
}

public sealed class SendResetPasswordUserRequestValidator : AbstractValidatorBase<SendResetPasswordUserRequest>
{
    public SendResetPasswordUserRequestValidator()
    {
        _ = RuleFor(x => x.UserId)
            .NotEmpty();
    }
}
