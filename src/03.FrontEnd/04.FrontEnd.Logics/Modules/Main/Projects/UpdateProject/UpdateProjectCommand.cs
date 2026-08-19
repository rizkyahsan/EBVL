using EBVL.Shared.Dto.Modules.Main.Projects.UpdateProject;

namespace EBVL.FrontEnd.Logics.Modules.Main.Projects.UpdateProject;

public sealed record UpdateProjectCommand : UpdateProjectRequest, IRequest { }

public sealed class UpdateProjectCommandValidator : AbstractValidatorBase<UpdateProjectCommand>
{
    public UpdateProjectCommandValidator()
    {
        Include(new UpdateProjectRequestValidator());
    }
}

public sealed class UpdateProjectCommandHandler(IBackEndApiService backEndApiService)
    : IRequestHandler<UpdateProjectCommand>
{
    public async Task Handle(UpdateProjectCommand request, CancellationToken cancellationToken)
    {
        var restRequest = new RestRequest(UpdateProjectRoute.ResourceUri(request.Id), Method.Patch);
        _ = restRequest.AddJsonBody(request);

        await backEndApiService.SendRequestAsync(restRequest, cancellationToken);
    }
}
