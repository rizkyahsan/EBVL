using EBVL.BackEnd.Logics.Modules.Main.MyProjects.GetMyProjects;
using EBVL.Shared.Dto.Modules.Main.MyProjects;
using EBVL.Shared.Dto.Modules.Main.MyProjects.GetMyProjects;

namespace EBVL.BackEnd.WebApi.Modules.Main.MyProjects.GetMyProjects;

public sealed class GetMyProjectsEndpoint : IEndpoint
{
    public RouteHandlerBuilder RegisterTo(WebApplication app)
    {
        return app
            .MapGet(GetMyProjectsRoute.Pattern, Handle)
            .RequireAuthorization()
            .WithTags(RouteConfig.Tag)
            .WithName(GetMyProjectsRoute.Name)
            .WithDescription(GetMyProjectsRoute.Description)
            .Produces<GetMyProjectsResponse>();
    }

    private static async Task<IResult> Handle(
        HttpContext context,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var query = new GetMyProjectsQuery();
        var response = await sender.Send(query, cancellationToken);

        return Results.Ok(response);
    }
}
