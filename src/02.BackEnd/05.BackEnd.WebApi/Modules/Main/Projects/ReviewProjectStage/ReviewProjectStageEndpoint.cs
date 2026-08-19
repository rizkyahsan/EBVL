using EBVL.BackEnd.Logics.Modules.Main.Projects.ReviewProjectStage;
using EBVL.Shared.Dto.Modules.Main.Projects;
using EBVL.Shared.Dto.Modules.Main.Projects.ReviewProjectStage;

namespace EBVL.BackEnd.WebApi.Modules.Main.Projects.ReviewProjectStage;

public sealed class ReviewProjectStageEndpoint : IEndpoint
{
    public RouteHandlerBuilder RegisterTo(WebApplication app)
    {
        return app
            .MapPatch(ReviewProjectStageRoute.Pattern, Handle)
            .RequireAuthorization()
            .WithTags(RouteConfig.Tag)
            .WithName(ReviewProjectStageRoute.Name)
            .WithDescription(ReviewProjectStageRoute.Description)
            .Produces(StatusCodes.Status204NoContent);
    }

    private static async Task<IResult> Handle(
        [FromRoute] Guid id,
        HttpContext context,
        ReviewProjectStageCommand command,
        ISender sender,
        CancellationToken cancellationToken)
    {
        if (id != command.Id)
        {
            throw ExceptionFor.Mismatch(nameof(id), id, nameof(command.Id), command.Id);
        }

        await sender.Send(command, cancellationToken);

        return Results.NoContent();
    }
}
