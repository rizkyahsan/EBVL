using EBVL.BackEnd.Logics.Modules.Examples.Dummies.GetDummies;
using EBVL.Shared.Dto.Modules.Examples.Dummies;
using EBVL.Shared.Dto.Modules.Examples.Dummies.GetDummies;

namespace EBVL.BackEnd.WebApi.Modules.Examples.Dummies.GetDummies;

public sealed class GetDummiesEndpoint : IEndpoint
{
    public RouteHandlerBuilder RegisterTo(WebApplication app)
    {
        return app
            .MapGet(GetDummiesRoute.Pattern, Handle)
            .AllowAnonymous()
            .WithTags(RouteConfig.Tag)
            .WithName(GetDummiesRoute.Name)
            .WithDescription(GetDummiesRoute.Description)
            .Produces<GetDummiesResponse>();
    }

    private static async Task<IResult> Handle(
        [AsParameters] GetDummiesQuery query,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var response = await sender.Send(query, cancellationToken);

        return Results.Ok(response);
    }
}
