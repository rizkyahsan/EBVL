
using EBVL.BackEnd.Logics.Modules.Administration.Audits.GetAudit;
using EBVL.Shared.Dto.Modules.Administration.Audits;
using EBVL.Shared.Dto.Modules.Administration.Audits.GetAudit;

namespace EBVL.BackEnd.WebApi.Modules.Administration.Audits.GetAudit;

public sealed class GetAuditEndpoint : IEndpoint
{
    public RouteHandlerBuilder RegisterTo(WebApplication app)
    {
        return app
            .MapGet(GetAuditRoute.Pattern, Handle)
            .RequireAuthorization()
            .WithTags(RouteConfig.Tag)
            .WithName(GetAuditRoute.Name)
            .WithDescription(GetAuditRoute.Description)
            .Produces<GetAuditResponse>();
    }

    private static async Task<IResult> Handle(
        [FromRoute] Guid auditId,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var query = new GetAuditQuery
        {
            AuditId = auditId
        };

        var response = await sender.Send(query, cancellationToken);

        return Results.Ok(response);
    }
}
