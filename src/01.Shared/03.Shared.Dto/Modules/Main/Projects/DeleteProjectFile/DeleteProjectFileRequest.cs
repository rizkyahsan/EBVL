namespace EBVL.Shared.Dto.Modules.Main.Projects.DeleteProjectFile;

public record DeleteProjectFileRequest
{
    public required Guid Id { get; set; }
}

public sealed class DeleteProjectFileRequestValidator : AbstractValidatorBase<DeleteProjectFileRequest>
{
    public DeleteProjectFileRequestValidator()
    {
        _ = RuleFor(x => x.Id)
            .NotEmpty();
    }
}
