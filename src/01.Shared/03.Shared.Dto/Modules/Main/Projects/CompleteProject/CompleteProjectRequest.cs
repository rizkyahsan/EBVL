namespace EBVL.Shared.Dto.Modules.Main.Projects.CompleteProject;

public record CompleteProjectRequest
{
    public required Guid Id { get; init; }

    public required List<CompleteProjectLenderRequest> ProjectLenders { get; set; }
}

public sealed class CompleteProjectRequestValidator : AbstractValidatorBase<CompleteProjectRequest>
{
    public CompleteProjectRequestValidator()
    {
        _ = RuleFor(x => x.Id)
            .NotEmpty();

        _ = RuleFor(x => x.ProjectLenders)
            .NotEmpty();

        _ = RuleForEach(m => m.ProjectLenders)
                .SetValidator(new CompleteProjectLenderRequestValidator());
    }
}
