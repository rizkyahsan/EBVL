using EBVL.BackEnd.Logics.Modules.Main.MyProjects.GetMyProject;
using EBVL.Shared.Dto.Modules.Main.MyProjects;
using EBVL.Shared.Dto.Modules.Main.MyProjects.GetMyProject;

namespace EBVL.BackEnd.WebApi.Modules.Main.MyProjects.GetMyProject;

public sealed class GetMyProjectEndpoint : IEndpoint
{
    public RouteHandlerBuilder RegisterTo(WebApplication app)
    {
        return app
            .MapGet(GetMyProjectRoute.Pattern, Handle)
            .RequireAuthorization()
            .WithTags(RouteConfig.Tag)
            .WithName(GetMyProjectRoute.Name)
            .WithDescription(GetMyProjectRoute.Description)
            .Produces<GetMyProjectResponse>();
    }

    private static async Task<IResult> Handle(
        [FromRoute] Guid id,
        HttpContext context,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var query = new GetMyProjectQuery { Id = id };

        var response = await sender.Send(query, cancellationToken);

        return Results.Ok(response);
    }
}
