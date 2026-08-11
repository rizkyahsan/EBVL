using EBVL.BackEnd.Logics.Modules.Administration.Audits.GetAudits;
using EBVL.Shared.Dto.Modules.Administration.Audits;
using EBVL.Shared.Dto.Modules.Administration.Audits.GetAudits;

namespace EBVL.BackEnd.WebApi.Modules.Administration.Audits.GetAudits;

public sealed class GetAuditsEndpoint : IEndpoint
{
    public RouteHandlerBuilder RegisterTo(WebApplication app)
    {
        return app
            .MapGet(GetAuditsRoute.Pattern, Handle)
            .RequireAuthorization()
            .WithTags(RouteConfig.Tag)
            .WithName(GetAuditsRoute.Name)
            .WithDescription(GetAuditsRoute.Description)
            .Produces<GetAuditsResponse>();
    }

    private static async Task<IResult> Handle(
        ISender sender,
        [AsParameters] GetAuditsQuery query,
        CancellationToken cancellationToken)
    {
        var response = await sender.Send(query, cancellationToken);

        return Results.Ok(response);
    }
}
