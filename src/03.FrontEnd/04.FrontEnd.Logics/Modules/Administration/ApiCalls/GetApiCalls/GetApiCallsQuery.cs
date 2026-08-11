using EBVL.Shared.Dto.Modules.Administration.ApiCalls.GetApiCalls;

namespace EBVL.FrontEnd.Logics.Modules.Administration.ApiCalls.GetApiCalls;

public sealed record GetApiCallsQuery : IRequest<GetApiCallsResponse>
{
}

public sealed class GetApiCallsQueryHandler(IBackEndApiService backEndApiService)
    : IRequestHandler<GetApiCallsQuery, GetApiCallsResponse>
{
    public async Task<GetApiCallsResponse> Handle(GetApiCallsQuery request, CancellationToken cancellationToken)
    {
        var restRequest = new RestRequest(GetApiCallsRoute.Pattern, Method.Get);

        return await backEndApiService.SendRequestAsync<GetApiCallsResponse>(restRequest, cancellationToken);
    }
}
