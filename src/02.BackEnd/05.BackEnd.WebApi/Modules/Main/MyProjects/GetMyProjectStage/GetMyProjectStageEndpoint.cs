using EBVL.BackEnd.Logics.Modules.Main.MyProjects.GetMyProjectStage;
using EBVL.Shared.Dto.Modules.Main.MyProjects;
using EBVL.Shared.Dto.Modules.Main.MyProjects.GetMyProjectStage;

namespace EBVL.BackEnd.WebApi.Modules.Main.MyProjects.GetMyProjectStage;

public sealed class GetMyProjectStageEndpoint : IEndpoint
{
    public RouteHandlerBuilder RegisterTo(WebApplication app)
    {
        return app
            .MapGet(GetMyProjectStageRoute.Pattern, Handle)
            .RequireAuthorization()
            .WithTags(RouteConfig.Tag)
            .WithName(GetMyProjectStageRoute.Name)
            .WithDescription(GetMyProjectStageRoute.Description)
            .Produces<GetMyProjectStageResponse>();
    }

    private static async Task<IResult> Handle(
        [FromRoute] Guid id,
        HttpContext context,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var query = new GetMyProjectStageQuery { Id = id };

        var response = await sender.Send(query, cancellationToken);

        return Results.Ok(response);
    }
}
