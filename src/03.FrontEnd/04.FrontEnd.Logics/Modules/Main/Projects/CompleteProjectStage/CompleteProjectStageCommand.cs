using EBVL.Shared.Dto.Modules.Main.Projects.CompleteProjectStage;

namespace EBVL.FrontEnd.Logics.Modules.Main.Projects.CompleteProjectStage;

public sealed record CompleteProjectStageCommand : CompleteProjectStageRequest, IRequest { }

public sealed class CompleteProjectStageCommandValidator : AbstractValidatorBase<CompleteProjectStageCommand>
{
    public CompleteProjectStageCommandValidator()
    {
        Include(new CompleteProjectStageRequestValidator());
    }
}

public sealed class CompleteProjectStageCommandHandler(IBackEndApiService backEndApiService)
    : IRequestHandler<CompleteProjectStageCommand>
{
    public async Task Handle(CompleteProjectStageCommand request, CancellationToken cancellationToken)
    {
        var restRequest = new RestRequest(CompleteProjectStageRoute.ResourceUri(request.Id), Method.Patch);
        _ = restRequest.AddJsonBody(request);

        await backEndApiService.SendRequestAsync(restRequest, cancellationToken);
    }
}
