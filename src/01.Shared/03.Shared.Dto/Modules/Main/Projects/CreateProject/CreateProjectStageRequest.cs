namespace EBVL.Shared.Dto.Modules.Main.Projects.CreateProject;

public record CreateProjectStageRequest
{
    public required string StageName { get; set; }
    public required string StageDesc { get; set; }
    public required DateTimeOffset? DueDate { get; set; }
}

public sealed class CreateProjectStageRequestValidator : AbstractValidatorBase<CreateProjectStageRequest>
{
    public CreateProjectStageRequestValidator()
    {
        _ = RuleFor(x => x.StageName)
            .NotEmpty()
            .MinimumLength(ProjectStagesMinimumLengthFor.Name)
            .MaximumLength(ProjectStagesMaximumLengthFor.Name);

        _ = RuleFor(x => x.StageDesc)
            .NotEmpty();

        _ = RuleFor(x => x.DueDate)
            .NotEmpty();
    }
}

