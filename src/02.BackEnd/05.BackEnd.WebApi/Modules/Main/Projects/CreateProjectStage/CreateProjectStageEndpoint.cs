using EBVL.BackEnd.Logics.Modules.Main.Projects.CreateProjectStage;
using EBVL.Shared.Dto.Modules.Main.Projects.CreateProjectStage;
using EBVL.Shared.Dto.Modules.Main.Projects.GetProjectStage;
using EBVL.Shared.Dto.Modules.Main.Projects;

namespace EBVL.BackEnd.WebApi.Modules.Main.Projects.CreateProjectStage;

public sealed class CreateProjectStageEndpoint : IEndpoint
{
    public RouteHandlerBuilder RegisterTo(WebApplication app)
    {
        return app
            .MapPost(CreateProjectStageRoute.Pattern, Handle)
            .RequireAuthorization()
            .WithTags(RouteConfig.Tag)
            .WithName(CreateProjectStageRoute.Name)
            .WithDescription(CreateProjectStageRoute.Description)
            .Produces<CreateProjectStageResponse>(StatusCodes.Status201Created);
    }

    private static async Task<IResult> Handle(
        [FromRoute] Guid id,
        HttpContext context,
        CreateProjectStageCommand command,
        ISender sender,
        CancellationToken cancellationToken)
    {
        if (id != command.ProjectId)
        {
            throw ExceptionFor.Mismatch(nameof(id), id, nameof(command.ProjectId), command.ProjectId);
        }

        var response = await sender.Send(command, cancellationToken);

        return Results.Created(GetProjectStageRoute.ResourceUri(response.Item.Id), response);
    }
}
