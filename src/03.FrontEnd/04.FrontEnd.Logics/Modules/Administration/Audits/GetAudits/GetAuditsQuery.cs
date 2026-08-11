using EBVL.Shared.Dto.Modules.Administration.Audits.GetAudits;

namespace EBVL.FrontEnd.Logics.Modules.Administration.Audits.GetAudits;

public sealed record GetAuditsQuery : GetAuditsRequest, IRequest<GetAuditsResponse>
{
}

public sealed class GetAuditsQueryValidator : AbstractValidatorBase<GetAuditsQuery>
{
    public GetAuditsQueryValidator()
    {
        Include(new GetAuditsRequestValidator());
    }
}

public sealed class GetAuditsQueryHandler(IBackEndApiService backEndApiService)
    : IRequestHandler<GetAuditsQuery, GetAuditsResponse>
{
    public async Task<GetAuditsResponse> Handle(GetAuditsQuery request, CancellationToken cancellationToken)
    {
        var restRequest = new RestRequest(GetAuditsRoute.ResourceUri, Method.Get);
        restRequest.AddQueryStringParameters(request);

        return await backEndApiService.SendRequestAsync<GetAuditsResponse>(restRequest, cancellationToken);
    }
}
