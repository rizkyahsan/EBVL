namespace EBVL.Shared.Dto.Modules.Main.Projects.CreateProjectStage;

public record CreateProjectStageRequest
{
    public required Guid ProjectId { get; set; }
    public required string Name { get; set; }
    public required string Desc { get; set; }
    public required DateTimeOffset? DueDate { get; set; }

    public required List<CreateProjectAttachmentRequest> ProjectAttachments { get; set; }

    public required List<CreateProjectReqRequest> ProjectReqs { get; set; }
}

public sealed class CreateProjectStageRequestValidator : AbstractValidatorBase<CreateProjectStageRequest>
{
    public CreateProjectStageRequestValidator()
    {
        _ = RuleFor(x => x.ProjectId)
            .NotEmpty();

        _ = RuleFor(x => x.Name)
            .NotEmpty()
            .MinimumLength(ProjectStagesMinimumLengthFor.Name)
            .MaximumLength(ProjectStagesMaximumLengthFor.Name);

        _ = RuleFor(x => x.Desc)
            .NotEmpty();

        _ = RuleFor(x => x.DueDate)
            .NotEmpty();

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
