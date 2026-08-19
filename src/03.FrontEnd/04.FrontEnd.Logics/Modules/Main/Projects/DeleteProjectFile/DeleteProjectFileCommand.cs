using EBVL.Shared.Dto.Modules.Main.Projects.DeleteProjectFile;

namespace EBVL.FrontEnd.Logics.Modules.Main.Projects.DeleteProjectFile;

public sealed record DeleteProjectFileCommand : DeleteProjectFileRequest, IRequest { }

public sealed class DeleteProjectFileCommandValidator : AbstractValidatorBase<DeleteProjectFileCommand>
{
    public DeleteProjectFileCommandValidator()
    {
        Include(new DeleteProjectFileRequestValidator());
    }
}

public sealed class DeleteProjectFileCommandHandler(IBackEndApiService backEndApiService)
    : IRequestHandler<DeleteProjectFileCommand>
{
    public async Task Handle(DeleteProjectFileCommand request, CancellationToken cancellationToken)
    {
        var restRequest = new RestRequest(DeleteProjectFileRoute.ResourceUri(request.Id), Method.Patch);

        await backEndApiService.SendRequestAsync(restRequest, cancellationToken);
    }
}
