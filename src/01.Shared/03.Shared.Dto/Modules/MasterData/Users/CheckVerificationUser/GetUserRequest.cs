namespace EBVL.Shared.Dto.Modules.MasterData.Users.CheckVerificationUser;

public record CheckVerificationUserRequest
{
    public required Guid UserId { get; init; }

    public required string Token { get; init; }
}

public sealed class CheckVerificationUserRequestValidator : AbstractValidatorBase<CheckVerificationUserRequest>
{
    public CheckVerificationUserRequestValidator()
    {
        _ = RuleFor(x => x.UserId)
            .NotEmpty();

        _ = RuleFor(x => x.Token)
            .NotEmpty();
    }
}
