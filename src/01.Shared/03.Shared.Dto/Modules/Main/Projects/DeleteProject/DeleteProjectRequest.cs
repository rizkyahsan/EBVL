namespace EBVL.Shared.Dto.Modules.Main.Projects.DeleteProject;

public record DeleteProjectRequest
{
    public required Guid ProjectId { get; init; }
}

public sealed class DeleteProjectRequestValidator : AbstractValidatorBase<DeleteProjectRequest>
{
    public DeleteProjectRequestValidator()
    {
        _ = RuleFor(x => x.ProjectId)
            .NotEmpty();
    }
}
