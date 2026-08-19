using EBVL.Shared.Dto.Modules.Main.Projects.GetProjectVerifies;

namespace EBVL.FrontEnd.Logics.Modules.Main.Projects.GetProjectVerifies;

public sealed record GetProjectVerifiesQuery : GetProjectVerifiesRequest, IRequest<GetProjectVerifiesResponse> { }

public sealed class GetProjectVerifiesQueryHandler(IBackEndApiService backEndApiService)
    : IRequestHandler<GetProjectVerifiesQuery, GetProjectVerifiesResponse>
{
    public async Task<GetProjectVerifiesResponse> Handle(GetProjectVerifiesQuery request, CancellationToken cancellationToken)
    {
        var restRequest = new RestRequest(GetProjectVerifiesRoute.ResourceUri, Method.Get);
        _ = restRequest.AddQueryParameter(nameof(request.LenderId), request.LenderId.ToString());

        return await backEndApiService.SendRequestAsync<GetProjectVerifiesResponse>(restRequest, cancellationToken);
    }
}
