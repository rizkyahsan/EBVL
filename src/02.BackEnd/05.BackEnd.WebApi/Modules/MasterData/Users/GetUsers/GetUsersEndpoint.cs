using EBVL.BackEnd.Logics.Modules.MasterData.Users.GetUsers;
using EBVL.Shared.Dto.Modules.MasterData.Users;
using EBVL.Shared.Dto.Modules.MasterData.Users.GetUsers;

namespace EBVL.BackEnd.WebApi.Modules.MasterData.Users.GetUsers;

public sealed class GetUsersEndpoint : IEndpoint
{
    public RouteHandlerBuilder RegisterTo(WebApplication app)
    {
        return app
            .MapGet(GetUsersRoute.Pattern, Handle)
            .RequireAuthorization()
            .WithTags(RouteConfig.Tag)
            .WithName(GetUsersRoute.Name)
            .WithDescription(GetUsersRoute.Description)
            .Produces<GetUsersResponse>();
    }

    private static async Task<IResult> Handle(
        ISender sender,
        CancellationToken cancellationToken)
    {
        var query = new GetUsersQuery();
        var response = await sender.Send(query, cancellationToken);

        return Results.Ok(response);
    }
}
