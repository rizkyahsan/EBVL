namespace EBVL.Shared.Dto.Modules.Main.Projects.CreateProject;

public record CreateProjectRequest
{
    public required string Title { get; set; }
    public required string Desc { get; set; }
    public required string Objective { get; set; }
    public required string FinanceType { get; set; }

    public required CreateProjectStageRequest ProjectStage { get; set; }

    public required List<CreateProjectLenderRequest> ProjectLenders { get; set; }

    public required List<CreateProjectAttachmentRequest> ProjectAttachments { get; set; }

    public required List<CreateProjectReqRequest> ProjectReqs { get; set; }
}

public sealed class CreateProjectRequestValidator : AbstractValidatorBase<CreateProjectRequest>
{
    public CreateProjectRequestValidator()
    {
        _ = RuleFor(x => x.Title)
            .NotEmpty()
            .MinimumLength(ProjectsMinimumLengthFor.Title)
            .MaximumLength(ProjectsMaximumLengthFor.Title);

        _ = RuleFor(x => x.Desc)
            .NotEmpty();

        _ = RuleFor(x => x.Objective)
            .NotEmpty()
            .MinimumLength(ProjectsMinimumLengthFor.Objective)
            .MaximumLength(ProjectsMaximumLengthFor.Objective);

        _ = RuleFor(x => x.FinanceType)
            .NotEmpty()
            .MinimumLength(ProjectsMinimumLengthFor.FinanceType)
            .MaximumLength(ProjectsMaximumLengthFor.FinanceType);

        _ = RuleFor(x => x.ProjectStage)
            .NotEmpty();

        _ = RuleFor(x => x.ProjectStage)
                .SetValidator(new CreateProjectStageRequestValidator());

        _ = RuleFor(x => x.ProjectLenders)
            .NotEmpty();

        _ = RuleForEach(m => m.ProjectLenders)
                .SetValidator(new CreateProjectLenderRequestValidator());

        _ = RuleFor(x => x.ProjectAttachments)
            .NotEmpty();

        _ = RuleForEach(m => m.ProjectAttachments)
                .SetValidator(new CreateProjectAttachmentRequestValidator());

        _ = RuleFor(x => x.ProjectReqs)
            .NotEmpty();

        _ = RuleForEach(m => m.ProjectReqs)
                .SetValidator(new CreateProjectReqRequestValidator());
    }
}
