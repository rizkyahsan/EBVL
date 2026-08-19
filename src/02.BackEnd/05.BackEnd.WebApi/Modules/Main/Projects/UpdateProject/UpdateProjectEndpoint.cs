using EBVL.BackEnd.Logics.Modules.Main.Projects.UpdateProject;
using EBVL.Shared.Dto.Modules.Main.Projects;
using EBVL.Shared.Dto.Modules.Main.Projects.UpdateProject;

namespace EBVL.BackEnd.WebApi.Modules.Main.Projects.UpdateProject;

public sealed class UpdateProjectEndpoint : IEndpoint
{
    public RouteHandlerBuilder RegisterTo(WebApplication app)
    {
        return app
            .MapPatch(UpdateProjectRoute.Pattern, Handle)
            .RequireAuthorization()
            .WithTags(RouteConfig.Tag)
            .WithName(UpdateProjectRoute.Name)
            .WithDescription(UpdateProjectRoute.Description)
            .Produces(StatusCodes.Status204NoContent);
    }

    private static async Task<IResult> Handle(
        [FromRoute] Guid id,
        HttpContext context,
        UpdateProjectCommand command,
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
