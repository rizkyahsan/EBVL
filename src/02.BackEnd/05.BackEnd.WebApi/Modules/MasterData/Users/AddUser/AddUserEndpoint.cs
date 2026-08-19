using EBVL.BackEnd.Logics.Modules.MasterData.Users.AddUser;
using EBVL.Shared.Dto.Modules.MasterData.Users;
using EBVL.Shared.Dto.Modules.MasterData.Users.AddUser;
using EBVL.Shared.Dto.Modules.MasterData.Users.GetUser;

namespace EBVL.BackEnd.WebApi.Modules.MasterData.Users.AddUser;

public sealed class AddUserEndpoint : IEndpoint
{
    public RouteHandlerBuilder RegisterTo(WebApplication app)
    {
        return app
            .MapPost(AddUserRoute.Pattern, Handle)
            .RequireAuthorization()
            .WithTags(RouteConfig.Tag)
            .WithName(AddUserRoute.Name)
            .WithDescription(AddUserRoute.Description)
            .Produces<AddUserResponse>(StatusCodes.Status201Created);
    }

    private static async Task<IResult> Handle(
        AddUserCommand command,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var response = await sender.Send(command, cancellationToken);

        return Results.Created(GetUserRoute.ResourceUri(response.Item.Id), response);
    }
}
