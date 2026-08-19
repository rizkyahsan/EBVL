using EBVL.Shared.Dto.Modules.MasterData.Lenders.GetLenders;

namespace EBVL.FrontEnd.Logics.Modules.MasterData.Lenders.GetLenders;

public sealed record GetLendersQuery : IRequest<GetLendersResponse>
{
}

public sealed class GetLendersQueryHandler(IBackEndApiService backEndApiService)
    : IRequestHandler<GetLendersQuery, GetLendersResponse>
{
    public async Task<GetLendersResponse> Handle(GetLendersQuery request, CancellationToken cancellationToken)
    {
        var restRequest = new RestRequest(GetLendersRoute.ResourceUri, Method.Get);

        return await backEndApiService.SendRequestAsync<GetLendersResponse>(restRequest, cancellationToken);
    }
}
