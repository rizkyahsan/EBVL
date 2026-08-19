namespace EBVL.Shared.Dto.Modules.MasterData.Users.SendOtpUser;

public record SendOtpUserRequest
{
    public required Guid UserId { get; set; }

    public required string Token { get; init; }
}

public sealed class SendOtpUserRequestValidator : AbstractValidatorBase<SendOtpUserRequest>
{
    public SendOtpUserRequestValidator()
    {
        _ = RuleFor(x => x.UserId)
            .NotEmpty();

        _ = RuleFor(x => x.Token)
            .NotEmpty();
    }
}
