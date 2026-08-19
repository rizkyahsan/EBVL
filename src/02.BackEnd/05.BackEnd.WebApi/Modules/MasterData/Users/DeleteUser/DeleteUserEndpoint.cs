using EBVL.BackEnd.Logics.Modules.MasterData.Users.DeleteUser;
using EBVL.Shared.Dto.Modules.MasterData.Users;
using EBVL.Shared.Dto.Modules.MasterData.Users.DeleteUser;

namespace EBVL.BackEnd.WebApi.Modules.MasterData.Users.DeleteUser;

public sealed class DeleteUserEndpoint : IEndpoint
{
    public RouteHandlerBuilder RegisterTo(WebApplication app)
    {
        return app
            .MapDelete(DeleteUserRoute.Pattern, Handle)
            .RequireAuthorization()
            .WithTags(RouteConfig.Tag)
            .WithName(DeleteUserRoute.Name)
            .WithDescription(DeleteUserRoute.Description)
            .Produces(StatusCodes.Status204NoContent);
    }

    private static async Task<IResult> Handle(
        [FromRoute] Guid userId,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new DeleteUserCommand
        {
            UserId = userId
        };

        await sender.Send(command, cancellationToken);

        return Results.NoContent();
    }
}
