using EBVL.Shared.Dto.Modules.Examples.Orders.GetOrders;

namespace EBVL.FrontEnd.Logics.Modules.Examples.Orders.GetOrders;

public sealed record GetOrdersQuery : IRequest<GetOrdersResponse>
{
}

public sealed class GetOrdersQueryHandler(IBackEndApiService backEndApiService)
    : IRequestHandler<GetOrdersQuery, GetOrdersResponse>
{
    public async Task<GetOrdersResponse> Handle(GetOrdersQuery request, CancellationToken cancellationToken)
    {
        var restRequest = new RestRequest(GetOrdersRoute.ResourceUri, Method.Get);

        return await backEndApiService.SendRequestAsync<GetOrdersResponse>(restRequest, cancellationToken);
    }
}
