namespace EBVL.Shared.Dto.Modules.Main.Projects.UpdateProject;

public record UpdateProjectRequest
{
    public required Guid Id { get; set; }

    public required string Title { get; set; }
    public required string Desc { get; set; }
    public required string Objective { get; set; }
    public required string FinanceType { get; set; }

    public required Guid StatusId { get; set; }

    public required List<UpdateProjectLenderRequest> ProjectLenders { get; set; }
}

public sealed class UpdateProjectRequestValidator : AbstractValidatorBase<UpdateProjectRequest>
{
    public UpdateProjectRequestValidator()
    {
        _ = RuleFor(x => x.Id)
            .NotEmpty();

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

        _ = RuleFor(x => x.StatusId)
            .NotEmpty();

        _ = RuleFor(x => x.ProjectLenders)
            .NotEmpty();

        _ = RuleForEach(m => m.ProjectLenders)
                .SetValidator(new UpdateProjectLenderRequestValidator());
    }
}
