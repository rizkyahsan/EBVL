namespace EBVL.Shared.Dto.Modules.MasterData.Lenders.DeleteLender;

public record DeleteLenderRequest
{
    public required Guid LenderId { get; init; }
}

public sealed class DeleteLenderRequestValidator : AbstractValidatorBase<DeleteLenderRequest>
{
    public DeleteLenderRequestValidator()
    {
        _ = RuleFor(x => x.LenderId)
            .NotEmpty();
    }
}
