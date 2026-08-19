using EBVL.Shared.Dto.Modules.Main.Projects.GetProject;

namespace EBVL.FrontEnd.Logics.Modules.Main.Projects.GetProject;

public sealed record GetProjectQuery : GetProjectRequest, IRequest<GetProjectResponse> { }

public sealed class GetProjectQueryValidator : AbstractValidatorBase<GetProjectQuery>
{
    public GetProjectQueryValidator()
    {
        Include(new GetProjectRequestValidator());
    }
}

public sealed class GetProjectQueryHandler(IBackEndApiService backEndApiService)
    : IRequestHandler<GetProjectQuery, GetProjectResponse>
{
    public async Task<GetProjectResponse> Handle(GetProjectQuery request, CancellationToken cancellationToken)
    {
        var restRequest = new RestRequest(GetProjectRoute.ResourceUri(request.Id), Method.Get);

        return await backEndApiService.SendRequestAsync<GetProjectResponse>(restRequest, cancellationToken);
    }
}
