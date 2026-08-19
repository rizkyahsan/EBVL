using EBVL.BackEnd.Logics.Modules.Log.LogEmails.GetLogEmails;
using EBVL.Shared.Dto.Modules.Log.LogEmails;
using EBVL.Shared.Dto.Modules.Log.LogEmails.GetLogEmails;

namespace EBVL.BackEnd.WebApi.Modules.Log.LogEmails.GetLogEmails;

public sealed class GetLogEmailsEndpoint : IEndpoint
{
    public RouteHandlerBuilder RegisterTo(WebApplication app)
    {
        return app
            .MapGet(GetLogEmailsRoute.Pattern, Handle)
            .RequireAuthorization()
            .WithTags(RouteConfig.Tag)
            .WithName(GetLogEmailsRoute.Name)
            .WithDescription(GetLogEmailsRoute.Description)
            .Produces<GetLogEmailsResponse>();
    }

    private static async Task<IResult> Handle(
        [AsParameters] GetLogEmailsQuery query,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var response = await sender.Send(query, cancellationToken);

        return Results.Ok(response);
    }
}
