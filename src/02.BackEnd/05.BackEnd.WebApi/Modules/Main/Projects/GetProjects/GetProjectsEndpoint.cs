using EBVL.BackEnd.Logics.Modules.Main.Projects.GetProjects;
using EBVL.Shared.Dto.Modules.Main.Projects;
using EBVL.Shared.Dto.Modules.Main.Projects.GetProjects;

namespace EBVL.BackEnd.WebApi.Modules.Main.Projects.GetProjects;

public sealed class GetProjectsEndpoint : IEndpoint
{
    public RouteHandlerBuilder RegisterTo(WebApplication app)
    {
        return app
            .MapGet(GetProjectsRoute.Pattern, Handle)
            .RequireAuthorization()
            .WithTags(RouteConfig.Tag)
            .WithName(GetProjectsRoute.Name)
            .WithDescription(GetProjectsRoute.Description)
            .Produces<GetProjectsResponse>();
    }

    private static async Task<IResult> Handle(
        [AsParameters] GetProjectsQuery query,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var response = await sender.Send(query, cancellationToken);

        return Results.Ok(response);
    }
}
