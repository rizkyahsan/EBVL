using EBVL.BackEnd.Logics.Modules.Main.Projects.PublishProjectStage;
using EBVL.Shared.Dto.Modules.Main.Projects;
using EBVL.Shared.Dto.Modules.Main.Projects.PublishProjectStage;

namespace EBVL.BackEnd.WebApi.Modules.Main.Projects.PublishProjectStage;

public sealed class PublishProjectStageEndpoint : IEndpoint
{
    public RouteHandlerBuilder RegisterTo(WebApplication app)
    {
        return app
            .MapPatch(PublishProjectStageRoute.Pattern, Handle)
            .RequireAuthorization()
            .WithTags(RouteConfig.Tag)
            .WithName(PublishProjectStageRoute.Name)
            .WithDescription(PublishProjectStageRoute.Description)
            .Produces(StatusCodes.Status204NoContent);
    }

    private static async Task<IResult> Handle(
        [FromRoute] Guid id,
        HttpContext context,
        PublishProjectStageCommand command,
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
