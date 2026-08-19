using EBVL.BackEnd.Logics.Modules.MasterData.Users.UpdateUserPic;
using EBVL.Shared.Dto.Modules.MasterData.Users;
using EBVL.Shared.Dto.Modules.MasterData.Users.UpdateUserPic;

namespace EBVL.BackEnd.WebApi.Modules.MasterData.Users.UpdateUserPic;

public sealed class UpdateUserPicEndpoint : IEndpoint
{
    public RouteHandlerBuilder RegisterTo(WebApplication app)
    {
        return app
            .MapPatch(UpdateUserPicRoute.Pattern, Handle)
            .RequireAuthorization()
            .WithTags(RouteConfig.Tag)
            .WithName(UpdateUserPicRoute.Name)
            .WithDescription(UpdateUserPicRoute.Description)
            .Produces(StatusCodes.Status204NoContent);
    }

    private static async Task<IResult> Handle(
        [FromRoute] Guid userId,
        UpdateUserPicCommand command,
        ISender sender,
        CancellationToken cancellationToken)
    {
        if (userId != command.UserId)
        {
            throw ExceptionFor.Mismatch(nameof(userId), userId, nameof(command.UserId), command.UserId);
        }

        await sender.Send(command, cancellationToken);

        return Results.NoContent();
    }
}
