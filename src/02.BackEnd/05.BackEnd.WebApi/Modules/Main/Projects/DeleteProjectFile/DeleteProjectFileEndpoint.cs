using EBVL.BackEnd.Logics.Modules.Main.Projects.DeleteProjectFile;
using EBVL.Shared.Dto.Modules.Main.Projects;
using EBVL.Shared.Dto.Modules.Main.Projects.DeleteProjectFile;

namespace EBVL.BackEnd.WebApi.Modules.Main.Projects.DeleteProjectFile;

public sealed class DeleteProjectFileEndpoint : IEndpoint
{
    public RouteHandlerBuilder RegisterTo(WebApplication app)
    {
        return app
            .MapPatch(DeleteProjectFileRoute.Pattern, Handle)
            .RequireAuthorization()
            .WithTags(RouteConfig.Tag)
            .WithName(DeleteProjectFileRoute.Name)
            .WithDescription(DeleteProjectFileRoute.Description)
            .Produces(StatusCodes.Status204NoContent);
    }

    private static async Task<IResult> Handle(
        [FromRoute] Guid id,
        HttpContext context,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new DeleteProjectFileCommand
        {
            Id = id
        };

        await sender.Send(command, cancellationToken);

        return Results.NoContent();
    }
}
