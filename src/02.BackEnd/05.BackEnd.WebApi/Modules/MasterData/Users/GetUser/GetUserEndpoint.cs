using EBVL.BackEnd.Logics.Modules.MasterData.Users.GetUser;
using EBVL.Shared.Dto.Modules.MasterData.Users;
using EBVL.Shared.Dto.Modules.MasterData.Users.GetUser;

namespace EBVL.BackEnd.WebApi.Modules.MasterData.Users.GetUser;

public sealed class GetUserEndpoint : IEndpoint
{
    public RouteHandlerBuilder RegisterTo(WebApplication app)
    {
        return app
            .MapGet(GetUserRoute.Pattern, Handle)
            .RequireAuthorization()
            .WithTags(RouteConfig.Tag)
            .WithName(GetUserRoute.Name)
            .WithDescription(GetUserRoute.Description)
            .Produces<GetUserResponse>();
    }

    private static async Task<IResult> Handle(
        [FromRoute] Guid userId,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var query = new GetUserQuery
        {
            UserId = userId
        };

        var response = await sender.Send(query, cancellationToken);

        return Results.Ok(response);
    }
}
