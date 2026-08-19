using EBVL.Shared.Dto.Modules.Main.Projects.GetProjectCompletes;

namespace EBVL.FrontEnd.Logics.Modules.Main.Projects.GetProjectCompletes;

public sealed record GetProjectCompletesQuery : GetProjectCompletesRequest, IRequest<GetProjectCompletesResponse> { }

public sealed class GetProjectCompletesQueryHandler(IBackEndApiService backEndApiService)
    : IRequestHandler<GetProjectCompletesQuery, GetProjectCompletesResponse>
{
    public async Task<GetProjectCompletesResponse> Handle(GetProjectCompletesQuery request, CancellationToken cancellationToken)
    {
        var restRequest = new RestRequest(GetProjectCompletesRoute.ResourceUri, Method.Get);
        _ = restRequest.AddQueryParameter(nameof(request.LenderId), request.LenderId.ToString());

        return await backEndApiService.SendRequestAsync<GetProjectCompletesResponse>(restRequest, cancellationToken);
    }
}
