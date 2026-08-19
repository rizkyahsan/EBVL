using EBVL.Shared.Dto.Modules.Main.Projects.ReviewProjectStage;

namespace EBVL.FrontEnd.Logics.Modules.Main.Projects.ReviewProjectStage;

public sealed record ReviewProjectStageCommand : ReviewProjectStageRequest, IRequest { }

public sealed class ReviewProjectStageCommandValidator : AbstractValidatorBase<ReviewProjectStageCommand>
{
    public ReviewProjectStageCommandValidator()
    {
        Include(new ReviewProjectStageRequestValidator());
    }
}

public sealed class ReviewProjectStageCommandHandler(IBackEndApiService backEndApiService)
    : IRequestHandler<ReviewProjectStageCommand>
{
    public async Task Handle(ReviewProjectStageCommand request, CancellationToken cancellationToken)
    {
        var restRequest = new RestRequest(ReviewProjectStageRoute.ResourceUri(request.Id), Method.Patch);
        _ = restRequest.AddJsonBody(request);

        await backEndApiService.SendRequestAsync(restRequest, cancellationToken);
    }
}
