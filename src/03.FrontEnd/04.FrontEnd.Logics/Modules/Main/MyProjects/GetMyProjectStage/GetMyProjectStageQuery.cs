using EBVL.Shared.Dto.Modules.Main.MyProjects.GetMyProjectStage;

namespace EBVL.FrontEnd.Logics.Modules.Main.MyProjects.GetMyProjectStage;

public sealed record GetMyProjectStageQuery : GetMyProjectStageRequest, IRequest<GetMyProjectStageResponse> { }

public sealed class GetMyProjectStageQueryValidator : AbstractValidatorBase<GetMyProjectStageQuery>
{
    public GetMyProjectStageQueryValidator()
    {
        Include(new GetMyProjectStageRequestValidator());
    }
}

public sealed class GetMyProjectStageQueryHandler(IBackEndApiService backEndApiService)
    : IRequestHandler<GetMyProjectStageQuery, GetMyProjectStageResponse>
{
    public async Task<GetMyProjectStageResponse> Handle(GetMyProjectStageQuery request, CancellationToken cancellationToken)
    {
        var restRequest = new RestRequest(GetMyProjectStageRoute.ResourceUri(request.Id), Method.Get);

        return await backEndApiService.SendRequestAsync<GetMyProjectStageResponse>(restRequest, cancellationToken);
    }
}
