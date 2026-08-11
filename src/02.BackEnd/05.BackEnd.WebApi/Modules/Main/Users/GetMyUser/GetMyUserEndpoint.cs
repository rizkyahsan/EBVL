using EBVL.BackEnd.Logics.Modules.Main.Users.GetMyUser;
using EBVL.Shared.Dto.Modules.Main.Users;
using EBVL.Shared.Dto.Modules.Main.Users.GetMyUser;

namespace EBVL.BackEnd.WebApi.Modules.Main.Users.GetMyUser;

public sealed class GetMyUserEndpoint : IEndpoint
{
    public RouteHandlerBuilder RegisterTo(WebApplication app)
    {
        return app
            .MapGet(GetMyUserRoute.Pattern, Handle)
            .RequireAuthorization()
            .WithTags(RouteConfig.Tag)
            .WithName(GetMyUserRoute.Name)
            .WithDescription(GetMyUserRoute.Description)
            .Produces<GetMyUserResponse>();
    }

    private static async Task<IResult> Handle(
        ISender sender,
        CancellationToken cancellationToken)
    {
        var query = new GetMyUserQuery();
        var response = await sender.Send(query, cancellationToken);

        return Results.Ok(response);
    }
}
