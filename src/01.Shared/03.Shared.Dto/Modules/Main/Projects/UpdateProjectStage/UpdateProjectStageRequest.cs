namespace EBVL.Shared.Dto.Modules.Main.Projects.UpdateProjectStage;

public record UpdateProjectStageRequest
{
    public required Guid Id { get; set; }

    public required string Name { get; set; }
    public required string Desc { get; set; }
    public required DateTimeOffset? DueDate { get; set; }

    public required List<UpdateProjectAttachmentRequest> ProjectAttachments { get; set; }

    public required List<UpdateProjectReqRequest> ProjectReqs { get; set; }
}

public sealed class UpdateProjectStageRequestValidator : AbstractValidatorBase<UpdateProjectStageRequest>
{
    public UpdateProjectStageRequestValidator()
    {
        _ = RuleFor(x => x.Id)
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
                .SetValidator(new UpdateProjectAttachmentRequestValidator());

        _ = RuleFor(x => x.ProjectReqs)
            .NotEmpty();

        _ = RuleForEach(m => m.ProjectReqs)
                .SetValidator(new UpdateProjectReqRequestValidator());
    }
}
