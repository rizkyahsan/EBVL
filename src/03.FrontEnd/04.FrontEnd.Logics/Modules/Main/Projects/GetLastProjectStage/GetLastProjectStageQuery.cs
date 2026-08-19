using EBVL.Shared.Dto.Modules.Main.Projects.GetLastProjectStage;

namespace EBVL.FrontEnd.Logics.Modules.Main.Projects.GetLastProjectStage;

public sealed record GetLastProjectStageQuery : GetLastProjectStageRequest, IRequest<GetLastProjectStageResponse> { }

public sealed class GetLastProjectStageQueryValidator : AbstractValidatorBase<GetLastProjectStageQuery>
{
    public GetLastProjectStageQueryValidator()
    {
        Include(new GetLastProjectStageRequestValidator());
    }
}

public sealed class GetLastProjectStageQueryHandler(IBackEndApiService backEndApiService)
    : IRequestHandler<GetLastProjectStageQuery, GetLastProjectStageResponse>
{
    public async Task<GetLastProjectStageResponse> Handle(GetLastProjectStageQuery request, CancellationToken cancellationToken)
    {
        var restRequest = new RestRequest(GetLastProjectStageRoute.ResourceUri(request.Id), Method.Get);

        return await backEndApiService.SendRequestAsync<GetLastProjectStageResponse>(restRequest, cancellationToken);
    }
}
