using EBVL.BackEnd.Logics.Modules.Main.Projects.UploadProjectFile;
using EBVL.Shared.Dto.Modules.Main.Projects;
using EBVL.Shared.Dto.Modules.Main.Projects.UploadProjectFile;

namespace EBVL.BackEnd.WebApi.Modules.Main.Projects.UploadProjectFile;

public sealed class UploadProjectFileEndpoint : IEndpoint
{
    public RouteHandlerBuilder RegisterTo(WebApplication app)
    {
        return app
            .MapPost(UploadProjectFileRoute.Pattern, Handle)
            .RequireAuthorization()
            .WithTags(RouteConfig.Tag)
            .WithName(UploadProjectFileRoute.Name)
            .WithDescription(UploadProjectFileRoute.Description)
            .Produces(StatusCodes.Status201Created);
    }

    private static async Task<IResult> Handle(
        [FromRoute] Guid id,
        HttpContext context,
        UploadProjectFileCommand command,
        ISender sender,
        CancellationToken cancellationToken)
    {
        if (id != command.Id)
        {
            throw ExceptionFor.Mismatch(nameof(id), id, nameof(command.Id), command.Id);
        }

        await sender.Send(command, cancellationToken);

        return Results.StatusCode(StatusCodes.Status201Created);
    }
}
