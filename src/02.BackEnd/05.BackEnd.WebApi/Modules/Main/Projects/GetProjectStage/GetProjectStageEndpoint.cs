using EBVL.BackEnd.Logics.Modules.Main.Projects.GetProjectStage;
using EBVL.Shared.Dto.Modules.Main.Projects;
using EBVL.Shared.Dto.Modules.Main.Projects.GetProjectStage;

namespace EBVL.BackEnd.WebApi.Modules.Main.Projects.GetProjectStage;

public sealed class GetProjectStageEndpoint : IEndpoint
{
    public RouteHandlerBuilder RegisterTo(WebApplication app)
    {
        return app
            .MapGet(GetProjectStageRoute.Pattern, Handle)
            .RequireAuthorization()
            .WithTags(RouteConfig.Tag)
            .WithName(GetProjectStageRoute.Name)
            .WithDescription(GetProjectStageRoute.Description)
            .Produces<GetProjectStageResponse>();
    }

    private static async Task<IResult> Handle(
        [FromRoute] Guid id,
        HttpContext context,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var query = new GetProjectStageQuery { Id = id };

        var response = await sender.Send(query, cancellationToken);

        return Results.Ok(response);
    }
}
