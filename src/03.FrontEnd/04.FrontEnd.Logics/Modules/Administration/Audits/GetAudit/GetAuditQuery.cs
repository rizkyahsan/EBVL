using EBVL.Shared.Dto.Modules.Administration.Audits.GetAudit;

namespace EBVL.FrontEnd.Logics.Modules.Administration.Audits.GetAudit;

public sealed record GetAuditQuery : GetAuditRequest, IRequest<GetAuditResponse>
{
}

public sealed class GetAuditQueryValidator : AbstractValidatorBase<GetAuditQuery>
{
    public GetAuditQueryValidator()
    {
        Include(new GetAuditRequestValidator());
    }
}

public sealed class GetAuditQueryHandler(IBackEndApiService backEndApiService)
    : IRequestHandler<GetAuditQuery, GetAuditResponse>
{
    public async Task<GetAuditResponse> Handle(GetAuditQuery request, CancellationToken cancellationToken)
    {
        var restRequest = new RestRequest(GetAuditRoute.ResourceUri(request.AuditId), Method.Get);

        return await backEndApiService.SendRequestAsync<GetAuditResponse>(restRequest, cancellationToken);
    }
}
