using EBVL.Shared.Dto.Modules.Main.MyProjects.UpdateMyProjectStage;

namespace EBVL.FrontEnd.Logics.Modules.Main.MyProjects.UpdateMyProjectStage;

public sealed record UpdateMyProjectStageCommand : UpdateMyProjectStageRequest, IRequest { }

public sealed class UpdateMyProjectStageCommandValidator : AbstractValidatorBase<UpdateMyProjectStageCommand>
{
    public UpdateMyProjectStageCommandValidator()
    {
        Include(new UpdateMyProjectStageRequestValidator());
    }
}

public sealed class UpdateMyProjectStageCommandHandler(IBackEndApiService backEndApiService)
    : IRequestHandler<UpdateMyProjectStageCommand>
{
    public async Task Handle(UpdateMyProjectStageCommand request, CancellationToken cancellationToken)
    {
        var restRequest = new RestRequest(UpdateMyProjectStageRoute.ResourceUri(request.Id), Method.Patch);
        _ = restRequest.AddJsonBody(request);

        await backEndApiService.SendRequestAsync(restRequest, cancellationToken);
    }
}
