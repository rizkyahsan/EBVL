namespace EBVL.Shared.Dto.Modules.MasterData.Users.UpdateUserPic;

public record UpdateUserPicRequest
{
    public required Guid UserId { get; set; }

    public required bool IsPic { get; set; } = false;
}

public sealed class UpdateUserPicRequestValidator : AbstractValidatorBase<UpdateUserPicRequest>
{
    public UpdateUserPicRequestValidator()
    {
        _ = RuleFor(x => x.UserId)
            .NotEmpty();
    }
}
