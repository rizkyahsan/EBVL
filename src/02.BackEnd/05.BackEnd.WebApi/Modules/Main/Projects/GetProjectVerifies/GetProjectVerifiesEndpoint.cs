using EBVL.BackEnd.Logics.Modules.Main.Projects.GetProjectVerifies;
using EBVL.Shared.Dto.Modules.Main.Projects;
using EBVL.Shared.Dto.Modules.Main.Projects.GetProjectVerifies;

namespace EBVL.BackEnd.WebApi.Modules.Main.Projects.GetProjectVerifies;

public sealed class GetProjectVerifiesEndpoint : IEndpoint
{
    public RouteHandlerBuilder RegisterTo(WebApplication app)
    {
        return app
            .MapGet(GetProjectVerifiesRoute.Pattern, Handle)
            .RequireAuthorization()
            .WithTags(RouteConfig.Tag)
            .WithName(GetProjectVerifiesRoute.Name)
            .WithDescription(GetProjectVerifiesRoute.Description)
            .Produces<GetProjectVerifiesResponse>();
    }

    private static async Task<IResult> Handle(
        [AsParameters] GetProjectVerifiesQuery query,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var response = await sender.Send(query, cancellationToken);

        return Results.Ok(response);
    }
}
