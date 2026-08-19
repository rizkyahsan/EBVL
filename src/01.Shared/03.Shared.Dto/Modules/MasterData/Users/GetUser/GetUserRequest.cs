namespace EBVL.Shared.Dto.Modules.MasterData.Users.GetUser;

public record GetUserRequest
{
    public required Guid UserId { get; init; }
}

public sealed class GetUserRequestValidator : AbstractValidatorBase<GetUserRequest>
{
    public GetUserRequestValidator()
    {
        _ = RuleFor(x => x.UserId)
            .NotEmpty();
    }
}
