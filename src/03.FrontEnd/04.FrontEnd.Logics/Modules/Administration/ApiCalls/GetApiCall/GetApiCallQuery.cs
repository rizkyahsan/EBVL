using EBVL.Shared.Dto.Modules.Administration.ApiCalls.GetApiCall;

namespace EBVL.FrontEnd.Logics.Modules.Administration.ApiCalls.GetApiCall;

public sealed record GetApiCallQuery : GetApiCallRequest, IRequest<GetApiCallResponse>
{
}

public sealed class GetApiCallQueryValidator : AbstractValidatorBase<GetApiCallQuery>
{
    public GetApiCallQueryValidator()
    {
        Include(new GetApiCallRequestValidator());
    }
}

public sealed class GetApiCallQueryHandler(IBackEndApiService backEndApiService)
    : IRequestHandler<GetApiCallQuery, GetApiCallResponse>
{
    public async Task<GetApiCallResponse> Handle(GetApiCallQuery request, CancellationToken cancellationToken)
    {
        var restRequest = new RestRequest(GetApiCallRoute.ResourceUri(request.ApiCallId), Method.Get);

        return await backEndApiService.SendRequestAsync<GetApiCallResponse>(restRequest, cancellationToken);
    }
}
