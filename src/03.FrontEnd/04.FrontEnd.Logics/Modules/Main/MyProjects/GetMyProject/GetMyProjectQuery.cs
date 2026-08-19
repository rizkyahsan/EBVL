using EBVL.Shared.Dto.Modules.Main.MyProjects.GetMyProject;

namespace EBVL.FrontEnd.Logics.Modules.Main.MyProjects.GetMyProject;

public sealed record GetMyProjectQuery : GetMyProjectRequest, IRequest<GetMyProjectResponse> { }

public sealed class GetMyProjectQueryValidator : AbstractValidatorBase<GetMyProjectQuery>
{
    public GetMyProjectQueryValidator()
    {
        Include(new GetMyProjectRequestValidator());
    }
}

public sealed class GetMyProjectQueryHandler(IBackEndApiService backEndApiService)
    : IRequestHandler<GetMyProjectQuery, GetMyProjectResponse>
{
    public async Task<GetMyProjectResponse> Handle(GetMyProjectQuery request, CancellationToken cancellationToken)
    {
        var restRequest = new RestRequest(GetMyProjectRoute.ResourceUri(request.Id), Method.Get);

        return await backEndApiService.SendRequestAsync<GetMyProjectResponse>(restRequest, cancellationToken);
    }
}
