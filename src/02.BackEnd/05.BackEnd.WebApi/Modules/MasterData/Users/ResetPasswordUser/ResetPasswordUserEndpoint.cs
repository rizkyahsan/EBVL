using EBVL.BackEnd.Logics.Modules.MasterData.Users.ResetPasswordUser;
using EBVL.Shared.Dto.Modules.MasterData.Users;
using EBVL.Shared.Dto.Modules.MasterData.Users.ResetPasswordUser;

namespace EBVL.BackEnd.WebApi.Modules.MasterData.Users.ResetPasswordUser;

public sealed class ResetPasswordUserEndpoint : IEndpoint
{
    public RouteHandlerBuilder RegisterTo(WebApplication app)
    {
        return app
            .MapPost(ResetPasswordUserRoute.Pattern, Handle)
            .AllowAnonymous()
            .WithTags(RouteConfig.Tag)
            .WithName(ResetPasswordUserRoute.Name)
            .WithDescription(ResetPasswordUserRoute.Description)
            .Produces<ResetPasswordUserResponse>();
    }

    private static async Task<IResult> Handle(
        [FromRoute] Guid userId,
        ResetPasswordUserCommand command,
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
