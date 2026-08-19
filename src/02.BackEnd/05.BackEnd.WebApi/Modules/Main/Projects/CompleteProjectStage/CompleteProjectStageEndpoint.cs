using EBVL.BackEnd.Logics.Modules.Main.Projects.CompleteProjectStage;
using EBVL.Shared.Dto.Modules.Main.Projects;
using EBVL.Shared.Dto.Modules.Main.Projects.CompleteProjectStage;

namespace EBVL.BackEnd.WebApi.Modules.Main.Projects.CompleteProjectStage;

public sealed class CompleteProjectStageEndpoint : IEndpoint
{
    public RouteHandlerBuilder RegisterTo(WebApplication app)
    {
        return app
            .MapPatch(CompleteProjectStageRoute.Pattern, Handle)
            .RequireAuthorization()
            .WithTags(RouteConfig.Tag)
            .WithName(CompleteProjectStageRoute.Name)
            .WithDescription(CompleteProjectStageRoute.Description)
            .Produces(StatusCodes.Status204NoContent);
    }

    private static async Task<IResult> Handle(
        [FromRoute] Guid id,
        HttpContext context,
        CompleteProjectStageCommand command,
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
