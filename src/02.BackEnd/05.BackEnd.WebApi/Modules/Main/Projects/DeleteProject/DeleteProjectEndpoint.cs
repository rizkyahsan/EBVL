using EBVL.BackEnd.Logics.Modules.Main.Projects.DeleteProject;
using EBVL.Shared.Dto.Modules.Main.Projects;
using EBVL.Shared.Dto.Modules.Main.Projects.DeleteProject;

namespace EBVL.BackEnd.WebApi.Modules.Main.Projects.DeleteProject;

public sealed class DeleteProjectEndpoint : IEndpoint
{
    public RouteHandlerBuilder RegisterTo(WebApplication app)
    {
        return app
            .MapPatch(DeleteProjectRoute.Pattern, Handle)
            .RequireAuthorization()
            .WithTags(RouteConfig.Tag)
            .WithName(DeleteProjectRoute.Name)
            .WithDescription(DeleteProjectRoute.Description)
            .Produces(StatusCodes.Status204NoContent);
    }

    private static async Task<IResult> Handle(
        [FromRoute] Guid projectId,
        HttpContext context,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new DeleteProjectCommand
        {
            ProjectId = projectId
        };

        await sender.Send(command, cancellationToken);

        return Results.NoContent();
    }
}
