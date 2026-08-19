using EBVL.BackEnd.Logics.Modules.Log.LogEmails.GetLogEmail;
using EBVL.Shared.Dto.Modules.Log.LogEmails;
using EBVL.Shared.Dto.Modules.Log.LogEmails.GetLogEmail;

namespace EBVL.BackEnd.WebApi.Modules.Log.LogEmails.GetLogEmail;

public sealed class GetLogEmailEndpoint : IEndpoint
{
    public RouteHandlerBuilder RegisterTo(WebApplication app)
    {
        return app
            .MapGet(GetLogEmailRoute.Pattern, Handle)
            .RequireAuthorization()
            .WithTags(RouteConfig.Tag)
            .WithName(GetLogEmailRoute.Name)
            .WithDescription(GetLogEmailRoute.Description)
            .Produces<GetLogEmailResponse>();
    }

    private static async Task<IResult> Handle(
        [FromRoute] Guid id,
        HttpContext context,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var query = new GetLogEmailQuery
        {
            Id = id
        };

        var response = await sender.Send(query, cancellationToken);

        return Results.Ok(response);
    }
}
