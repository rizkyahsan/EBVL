using EBVL.BackEnd.Logics.Modules.Main.MyProjects.UpdateMyProjectStage;
using EBVL.Shared.Dto.Modules.Main.MyProjects;
using EBVL.Shared.Dto.Modules.Main.MyProjects.UpdateMyProjectStage;

namespace EBVL.BackEnd.WebApi.Modules.Main.MyProjects.UpdateMyProjectStage;

public sealed class UpdateMyProjectStageEndpoint : IEndpoint
{
    public RouteHandlerBuilder RegisterTo(WebApplication app)
    {
        return app
            .MapPatch(UpdateMyProjectStageRoute.Pattern, Handle)
            .RequireAuthorization()
            .WithTags(RouteConfig.Tag)
            .WithName(UpdateMyProjectStageRoute.Name)
            .WithDescription(UpdateMyProjectStageRoute.Description)
            .Produces(StatusCodes.Status204NoContent);
    }

    private static async Task<IResult> Handle(
        [FromRoute] Guid id,
        HttpContext context,
        UpdateMyProjectStageCommand command,
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
