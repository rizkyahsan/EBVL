using EBVL.Shared.Dto.Modules.Main.Projects.UpdateProjectStage;

namespace EBVL.FrontEnd.Logics.Modules.Main.Projects.UpdateProjectStage;

public sealed record UpdateProjectStageCommand : UpdateProjectStageRequest, IRequest { }

public sealed class UpdateProjectStageCommandValidator : AbstractValidatorBase<UpdateProjectStageCommand>
{
    public UpdateProjectStageCommandValidator()
    {
        Include(new UpdateProjectStageRequestValidator());
    }
}

public sealed class UpdateProjectStageCommandHandler(IBackEndApiService backEndApiService)
    : IRequestHandler<UpdateProjectStageCommand>
{
    public async Task Handle(UpdateProjectStageCommand request, CancellationToken cancellationToken)
    {
        var restRequest = new RestRequest(UpdateProjectStageRoute.ResourceUri(request.Id), Method.Patch);
        _ = restRequest.AddJsonBody(request);

        await backEndApiService.SendRequestAsync(restRequest, cancellationToken);
    }
}
