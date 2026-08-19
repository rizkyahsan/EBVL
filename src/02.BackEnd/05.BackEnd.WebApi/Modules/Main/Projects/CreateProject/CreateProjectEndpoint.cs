using EBVL.BackEnd.Logics.Modules.Main.Projects.CreateProject;
using EBVL.Shared.Dto.Modules.Main.Projects.CreateProject;
using EBVL.Shared.Dto.Modules.Main.Projects.GetProject;
using EBVL.Shared.Dto.Modules.Main.Projects;

namespace EBVL.BackEnd.WebApi.Modules.Main.Projects.CreateProject;

public sealed class CreateProjectEndpoint : IEndpoint
{
    public RouteHandlerBuilder RegisterTo(WebApplication app)
    {
        return app
            .MapPost(CreateProjectRoute.Pattern, Handle)
            .RequireAuthorization()
            .WithTags(RouteConfig.Tag)
            .WithName(CreateProjectRoute.Name)
            .WithDescription(CreateProjectRoute.Description)
            .Produces<CreateProjectResponse>(StatusCodes.Status201Created);
    }

    private static async Task<IResult> Handle(
        HttpContext context,
        CreateProjectCommand command,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var response = await sender.Send(command, cancellationToken);

        return Results.Created(GetProjectRoute.ResourceUri(response.Item.Id), response);
    }
}
