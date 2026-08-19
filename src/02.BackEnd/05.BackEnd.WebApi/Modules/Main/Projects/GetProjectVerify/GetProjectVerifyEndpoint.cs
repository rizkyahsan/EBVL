using EBVL.BackEnd.Logics.Modules.Main.Projects.GetProjectVerify;
using EBVL.Shared.Dto.Modules.Main.Projects;
using EBVL.Shared.Dto.Modules.Main.Projects.GetProjectVerify;

namespace EBVL.BackEnd.WebApi.Modules.Main.Projects.GetProjectVerify;

public sealed class GetProjectVerifyEndpoint : IEndpoint
{
    public RouteHandlerBuilder RegisterTo(WebApplication app)
    {
        return app
            .MapGet(GetProjectVerifyRoute.Pattern, Handle)
            .RequireAuthorization()
            .WithTags(RouteConfig.Tag)
            .WithName(GetProjectVerifyRoute.Name)
            .WithDescription(GetProjectVerifyRoute.Description)
            .Produces<GetProjectVerifyResponse>();
    }

    private static async Task<IResult> Handle(
        [FromRoute] Guid id,
        HttpContext context,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var query = new GetProjectVerifyQuery { Id = id };

        var response = await sender.Send(query, cancellationToken);

        return Results.Ok(response);
    }
}
