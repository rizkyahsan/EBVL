using EBVL.BackEnd.Logics.Modules.Main.Projects.GetProjectCompletes;
using EBVL.Shared.Dto.Modules.Main.Projects;
using EBVL.Shared.Dto.Modules.Main.Projects.GetProjectCompletes;

namespace EBVL.BackEnd.WebApi.Modules.Main.Projects.GetProjectCompletes;

public sealed class GetProjectCompletesEndpoint : IEndpoint
{
    public RouteHandlerBuilder RegisterTo(WebApplication app)
    {
        return app
            .MapGet(GetProjectCompletesRoute.Pattern, Handle)
            .RequireAuthorization()
            .WithTags(RouteConfig.Tag)
            .WithName(GetProjectCompletesRoute.Name)
            .WithDescription(GetProjectCompletesRoute.Description)
            .Produces<GetProjectCompletesResponse>();
    }

    private static async Task<IResult> Handle(
        [AsParameters] GetProjectCompletesQuery query,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var response = await sender.Send(query, cancellationToken);

        return Results.Ok(response);
    }
}
