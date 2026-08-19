using EBVL.Shared.Dto.Modules.Main.Projects.GetProjects;

namespace EBVL.FrontEnd.Logics.Modules.Main.Projects.GetProjects;

public sealed record GetProjectsQuery : GetProjectsRequest, IRequest<GetProjectsResponse> { }

public sealed class GetProjectsQueryHandler(IBackEndApiService backEndApiService)
    : IRequestHandler<GetProjectsQuery, GetProjectsResponse>
{
    public async Task<GetProjectsResponse> Handle(GetProjectsQuery request, CancellationToken cancellationToken)
    {
        var restRequest = new RestRequest(GetProjectsRoute.ResourceUri, Method.Get);
        _ = restRequest.AddQueryParameter(nameof(request.LenderId), request.LenderId.ToString());
        _ = restRequest.AddQueryParameter(nameof(request.StatusCode), request.StatusCode);

        return await backEndApiService.SendRequestAsync<GetProjectsResponse>(restRequest, cancellationToken);
    }
}
