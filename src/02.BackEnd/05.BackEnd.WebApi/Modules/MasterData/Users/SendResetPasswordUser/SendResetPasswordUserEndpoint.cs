using EBVL.BackEnd.Logics.Modules.MasterData.Users.SendResetPasswordUser;
using EBVL.Shared.Dto.Modules.MasterData.Users;
using EBVL.Shared.Dto.Modules.MasterData.Users.SendResetPasswordUser;

namespace EBVL.BackEnd.WebApi.Modules.MasterData.Users.SendResetPasswordUser;

public sealed class SendResetPasswordUserEndpoint : IEndpoint
{
    public RouteHandlerBuilder RegisterTo(WebApplication app)
    {
        return app
            .MapPost(SendResetPasswordUserRoute.Pattern, Handle)
            .RequireAuthorization()
            .WithTags(RouteConfig.Tag)
            .WithName(SendResetPasswordUserRoute.Name)
            .WithDescription(SendResetPasswordUserRoute.Description)
            .Produces<SendResetPasswordUserResponse>();
    }

    private static async Task<IResult> Handle(
        [FromRoute] Guid userId,
        SendResetPasswordUserCommand command,
        ISender sender,
        CancellationToken cancellationToken)
    {
        if (userId != command.UserId)
        {
            throw ExceptionFor.Mismatch(nameof(userId), userId, nameof(command.UserId), command.UserId);
        }

        var response = await sender.Send(command, cancellationToken);

        return Results.Ok(response);
    }
}
