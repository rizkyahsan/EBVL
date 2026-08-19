using EBVL.Shared.Dto.Modules.Main.Projects.DeleteProject;

namespace EBVL.BackEnd.Logics.Modules.Main.Projects.DeleteProject;

[AuthorizeRequest]
public sealed record DeleteProjectCommand : DeleteProjectRequest, IRequest
{
}

public sealed class DeleteProjectCommandValidator : AbstractValidatorBase<DeleteProjectCommand>
{
    public DeleteProjectCommandValidator()
    {
        Include(new DeleteProjectRequestValidator());
    }
}

public sealed class DeleteProjectCommandHandler(IDatabaseService databaseService)
    : IRequestHandler<DeleteProjectCommand>
{
    public async Task Handle(DeleteProjectCommand request, CancellationToken cancellationToken)
    {
        var project = await databaseService.Projects
            .Where(x => !x.IsDeleted && x.Id == request.ProjectId)
            .SingleOrDefaultAsync(cancellationToken)
            ?? throw ExceptionFor.EntityNotFound(ProjectsDisplayTextFor.Project, CommonDisplayTextFor.Id, request.ProjectId);

        project.IsDeleted = true;

        _ = await databaseService.SaveAsync(nameof(DeleteProject), cancellationToken);
    }
}
