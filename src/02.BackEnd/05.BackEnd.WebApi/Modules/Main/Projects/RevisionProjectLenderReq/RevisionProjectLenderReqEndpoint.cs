using EBVL.BackEnd.Logics.Modules.Main.Projects.RevisionProjectLenderReq;
using EBVL.Shared.Dto.Modules.Main.Projects;
using EBVL.Shared.Dto.Modules.Main.Projects.RevisionProjectLenderReq;

namespace EBVL.BackEnd.WebApi.Modules.Main.Projects.RevisionProjectLenderReq;

public sealed class RevisionProjectLenderReqEndpoint : IEndpoint
{
    public RouteHandlerBuilder RegisterTo(WebApplication app)
    {
        return app
            .MapPatch(RevisionProjectLenderReqRoute.Pattern, Handle)
            .RequireAuthorization()
            .WithTags(RouteConfig.Tag)
            .WithName(RevisionProjectLenderReqRoute.Name)
            .WithDescription(RevisionProjectLenderReqRoute.Description)
            .Produces(StatusCodes.Status204NoContent);
    }

    private static async Task<IResult> Handle(
        [FromRoute] Guid id,
        HttpContext context,
        RevisionProjectLenderReqCommand command,
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
