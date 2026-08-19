using EBVL.Shared.Dto.Modules.Main.Projects.GetProjectStage;

namespace EBVL.FrontEnd.Logics.Modules.Main.Projects.GetProjectStage;

public sealed record GetProjectStageQuery : GetProjectStageRequest, IRequest<GetProjectStageResponse> { }

public sealed class GetProjectStageQueryValidator : AbstractValidatorBase<GetProjectStageQuery>
{
    public GetProjectStageQueryValidator()
    {
        Include(new GetProjectStageRequestValidator());
    }
}

public sealed class GetProjectStageQueryHandler(IBackEndApiService backEndApiService)
    : IRequestHandler<GetProjectStageQuery, GetProjectStageResponse>
{
    public async Task<GetProjectStageResponse> Handle(GetProjectStageQuery request, CancellationToken cancellationToken)
    {
        var restRequest = new RestRequest(GetProjectStageRoute.ResourceUri(request.Id), Method.Get);

        return await backEndApiService.SendRequestAsync<GetProjectStageResponse>(restRequest, cancellationToken);
    }
}
