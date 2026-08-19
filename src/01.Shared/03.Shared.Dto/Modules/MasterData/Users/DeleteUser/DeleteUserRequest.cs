namespace EBVL.Shared.Dto.Modules.MasterData.Users.DeleteUser;

public record DeleteUserRequest
{
    public required Guid UserId { get; init; }
}

public sealed class DeleteUserRequestValidator : AbstractValidatorBase<DeleteUserRequest>
{
    public DeleteUserRequestValidator()
    {
        _ = RuleFor(x => x.UserId)
            .NotEmpty();
    }
}
