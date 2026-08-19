namespace EBVL.Shared.Dto.Modules.Main.Projects.ReviewProjectStage;

public record ReviewProjectStageRequest
{
    public required Guid Id { get; set; }
}

public sealed class ReviewProjectStageRequestValidator : AbstractValidatorBase<ReviewProjectStageRequest>
{
    public ReviewProjectStageRequestValidator()
    {
        _ = RuleFor(x => x.Id)
            .NotEmpty();
    }
}
