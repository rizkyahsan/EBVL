using EBVL.Shared.Dto.Modules.Main.MyProjects.GetMyProjects;

namespace EBVL.FrontEnd.Logics.Modules.Main.MyProjects.GetMyProjects;

public sealed record GetMyProjectsQuery : IRequest<GetMyProjectsResponse> { }

public sealed class GetMyProjectsQueryHandler(IBackEndApiService backEndApiService)
    : IRequestHandler<GetMyProjectsQuery, GetMyProjectsResponse>
{
    public async Task<GetMyProjectsResponse> Handle(GetMyProjectsQuery request, CancellationToken cancellationToken)
    {
        var restRequest = new RestRequest(GetMyProjectsRoute.ResourceUri, Method.Get);

        return await backEndApiService.SendRequestAsync<GetMyProjectsResponse>(restRequest, cancellationToken);
    }
}
