namespace EBVL.Shared.Dto.Modules.Main.Projects.CompleteProjectStage;

public record CompleteProjectStageRequest
{
    public required Guid Id { get; init; }

    public required List<CompleteProjectLenderReqRequest> ProjectLenderReqs { get; init; }
}

public sealed class CompleteProjectStageRequestValidator : AbstractValidatorBase<CompleteProjectStageRequest>
{
    public CompleteProjectStageRequestValidator()
    {
        _ = RuleFor(x => x.Id)
            .NotEmpty();

        _ = RuleFor(x => x.ProjectLenderReqs)
            .NotEmpty();

        _ = RuleForEach(m => m.ProjectLenderReqs)
                .SetValidator(new CompleteProjectLenderReqRequestValidator());
    }
}
