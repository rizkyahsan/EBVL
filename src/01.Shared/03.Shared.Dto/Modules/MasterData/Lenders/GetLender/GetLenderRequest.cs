namespace EBVL.Shared.Dto.Modules.MasterData.Lenders.GetLender;

public record GetLenderRequest
{
    public required Guid LenderId { get; init; }
}

public sealed class GetLenderRequestValidator : AbstractValidatorBase<GetLenderRequest>
{
    public GetLenderRequestValidator()
    {
        _ = RuleFor(x => x.LenderId)
            .NotEmpty();
    }
}
