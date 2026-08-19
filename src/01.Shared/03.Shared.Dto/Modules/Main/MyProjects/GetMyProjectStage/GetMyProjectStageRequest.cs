namespace EBVL.Shared.Dto.Modules.Main.MyProjects.GetMyProjectStage;

public record GetMyProjectStageRequest
{
    public required Guid Id { get; init; }
}

public sealed class GetMyProjectStageRequestValidator : AbstractValidatorBase<GetMyProjectStageRequest>
{
    public GetMyProjectStageRequestValidator()
    {
        _ = RuleFor(x => x.Id)
            .NotEmpty();
    }
}
