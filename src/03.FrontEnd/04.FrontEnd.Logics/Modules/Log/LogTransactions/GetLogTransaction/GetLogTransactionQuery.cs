using EBVL.Shared.Dto.Modules.Log.LogTransactions.GetLogTransaction;

namespace EBVL.FrontEnd.Logics.Modules.Log.LogTransactions.GetLogTransaction;

public sealed record GetLogTransactionQuery : GetLogTransactionRequest, IRequest<GetLogTransactionResponse> { }

public sealed class GetLogTransactionQueryValidator : AbstractValidatorBase<GetLogTransactionQuery>
{
    public GetLogTransactionQueryValidator()
    {
        Include(new GetLogTransactionRequestValidator());
    }
}

public sealed class GetLogTransactionQueryHandler(IBackEndApiService backEndApiService)
    : IRequestHandler<GetLogTransactionQuery, GetLogTransactionResponse>
{
    public async Task<GetLogTransactionResponse> Handle(GetLogTransactionQuery request, CancellationToken cancellationToken)
    {
        var restRequest = new RestRequest(GetLogTransactionRoute.ResourceUri(request.Id), Method.Get);

        return await backEndApiService.SendRequestAsync<GetLogTransactionResponse>(restRequest, cancellationToken);
    }
}
