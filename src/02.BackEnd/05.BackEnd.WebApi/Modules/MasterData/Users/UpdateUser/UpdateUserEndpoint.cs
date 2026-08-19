using EBVL.BackEnd.Logics.Modules.MasterData.Users.UpdateUser;
using EBVL.Shared.Dto.Modules.MasterData.Users;
using EBVL.Shared.Dto.Modules.MasterData.Users.UpdateUser;

namespace EBVL.BackEnd.WebApi.Modules.MasterData.Users.UpdateUser;

public sealed class UpdateUserEndpoint : IEndpoint
{
    public RouteHandlerBuilder RegisterTo(WebApplication app)
    {
        return app
            .MapPatch(UpdateUserRoute.Pattern, Handle)
            .RequireAuthorization()
            .WithTags(RouteConfig.Tag)
            .WithName(UpdateUserRoute.Name)
            .WithDescription(UpdateUserRoute.Description)
            .Produces(StatusCodes.Status204NoContent);
    }

    private static async Task<IResult> Handle(
        [FromRoute] Guid userId,
        UpdateUserCommand command,
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
