using EBVL.Shared.Dto.Modules.MasterData.Lenders.GetLender;

namespace EBVL.FrontEnd.Logics.Modules.MasterData.Lenders.GetLender;

public sealed record GetLenderQuery : GetLenderRequest, IRequest<GetLenderResponse>
{
}

public sealed class GetLenderQueryValidator : AbstractValidatorBase<GetLenderQuery>
{
    public GetLenderQueryValidator()
    {
        Include(new GetLenderRequestValidator());
    }
}

public sealed class GetLenderQueryHandler(IBackEndApiService backEndApiService)
    : IRequestHandler<GetLenderQuery, GetLenderResponse>
{
    public async Task<GetLenderResponse> Handle(GetLenderQuery request, CancellationToken cancellationToken)
    {
        var restRequest = new RestRequest(GetLenderRoute.ResourceUri(request.LenderId), Method.Get);

        return await backEndApiService.SendRequestAsync<GetLenderResponse>(restRequest, cancellationToken);
    }
}
