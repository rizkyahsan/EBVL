namespace EBVL.Shared.Dto.Modules.Main.Projects.GetProjectStage;

public record GetProjectStageRequest
{
    public required Guid Id { get; init; }
}

public sealed class GetProjectStageRequestValidator : AbstractValidatorBase<GetProjectStageRequest>
{
    public GetProjectStageRequestValidator()
    {
        _ = RuleFor(x => x.Id)
            .NotEmpty();
    }
}
