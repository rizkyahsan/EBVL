using EBVL.Shared.Dto.Modules.Main.Projects.PublishProjectStage;

namespace EBVL.FrontEnd.Logics.Modules.Main.Projects.PublishProjectStage;

public sealed record PublishProjectStageCommand : PublishProjectStageRequest, IRequest { }

public sealed class PublishProjectStageCommandValidator : AbstractValidatorBase<PublishProjectStageCommand>
{
    public PublishProjectStageCommandValidator()
    {
        Include(new PublishProjectStageRequestValidator());
    }
}

public sealed class PublishProjectStageCommandHandler(IBackEndApiService backEndApiService)
    : IRequestHandler<PublishProjectStageCommand>
{
    public async Task Handle(PublishProjectStageCommand request, CancellationToken cancellationToken)
    {
        var restRequest = new RestRequest(PublishProjectStageRoute.ResourceUri(request.Id), Method.Patch);
        _ = restRequest.AddJsonBody(request);

        await backEndApiService.SendRequestAsync(restRequest, cancellationToken);
    }
}
