using EBVL.Shared.Dto.Modules.Administration;
using EBVL.Shared.Dto.Modules.Administration.Audits.GetAudit;

namespace EBVL.BackEnd.Logics.Modules.Administration.Audits.GetAudit;

[AuthorizeRequestByPermission(Permissions.AdministrationAuditsRead)]
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

public sealed class GetAuditQueryHandler(IDatabaseService databaseService)
    : IRequestHandler<GetAuditQuery, GetAuditResponse>
{
    public async Task<GetAuditResponse> Handle(GetAuditQuery request, CancellationToken cancellationToken)
    {
        var audit = await databaseService.Audits
            .AsNoTracking()
            .Where(audit => audit.Id == request.AuditId)
            .Select(audit => audit.ToAuditItem<AuditItem>())
            .SingleOrDefaultAsync(cancellationToken)
            ?? throw ExceptionFor.EntityNotFound(AuditsDisplayTextFor.Audit, CommonDisplayTextFor.Id, request.AuditId);

        return new GetAuditResponse
        {
            Item = audit
        };
    }
}
