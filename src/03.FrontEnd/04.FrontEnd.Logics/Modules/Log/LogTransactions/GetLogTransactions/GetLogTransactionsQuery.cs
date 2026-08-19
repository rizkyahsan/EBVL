using EBVL.Shared.Dto.Modules.Log.LogTransactions.GetLogTransactions;

namespace EBVL.FrontEnd.Logics.Modules.Log.LogTransactions.GetLogTransactions;

public sealed record GetLogTransactionsQuery : GetLogTransactionsRequest, IRequest<GetLogTransactionsResponse> { }

public sealed class GetLogTransactionsQueryHandler(IBackEndApiService backEndApiService)
    : IRequestHandler<GetLogTransactionsQuery, GetLogTransactionsResponse>
{
    public async Task<GetLogTransactionsResponse> Handle(GetLogTransactionsQuery request, CancellationToken cancellationToken)
    {
        var restRequest = new RestRequest(GetLogTransactionsRoute.ResourceUri, Method.Get);
        _ = restRequest.AddQueryParameter(nameof(request.ProjectId), request.ProjectId.ToString());
        _ = restRequest.AddQueryParameter(nameof(request.ProjectStageId), request.ProjectStageId.ToString() ?? null);
        _ = restRequest.AddQueryParameter(nameof(request.ProjectLenderId), request.ProjectLenderId.ToString() ?? null);

        return await backEndApiService.SendRequestAsync<GetLogTransactionsResponse>(restRequest, cancellationToken);
    }
}
