namespace EBVL.Shared.Dto.Modules.Main.Projects.GetLastProjectStage;

public record GetLastProjectStageRequest
{
    public required Guid Id { get; init; }
}

public sealed class GetLastProjectStageRequestValidator : AbstractValidatorBase<GetLastProjectStageRequest>
{
    public GetLastProjectStageRequestValidator()
    {
        _ = RuleFor(x => x.Id)
            .NotEmpty();
    }
}
