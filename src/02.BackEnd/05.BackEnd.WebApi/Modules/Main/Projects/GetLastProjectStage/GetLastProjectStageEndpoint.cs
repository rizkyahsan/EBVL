using EBVL.BackEnd.Logics.Modules.Main.Projects.GetLastProjectStage;
using EBVL.Shared.Dto.Modules.Main.Projects;
using EBVL.Shared.Dto.Modules.Main.Projects.GetLastProjectStage;

namespace EBVL.BackEnd.WebApi.Modules.Main.Projects.GetLastProjectStage;

public sealed class GetLastProjectStageEndpoint : IEndpoint
{
    public RouteHandlerBuilder RegisterTo(WebApplication app)
    {
        return app
            .MapGet(GetLastProjectStageRoute.Pattern, Handle)
            .RequireAuthorization()
            .WithTags(RouteConfig.Tag)
            .WithName(GetLastProjectStageRoute.Name)
            .WithDescription(GetLastProjectStageRoute.Description)
            .Produces<GetLastProjectStageResponse>();
    }

    private static async Task<IResult> Handle(
        [FromRoute] Guid id,
        HttpContext context,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var query = new GetLastProjectStageQuery { Id = id };

        var response = await sender.Send(query, cancellationToken);

        return Results.Ok(response);
    }
}
