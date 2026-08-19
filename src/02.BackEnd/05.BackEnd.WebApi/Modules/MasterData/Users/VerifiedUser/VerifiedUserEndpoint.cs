using EBVL.BackEnd.Logics.Modules.MasterData.Users.VerifiedUser;
using EBVL.Shared.Dto.Modules.MasterData.Users;
using EBVL.Shared.Dto.Modules.MasterData.Users.VerifiedUser;

namespace EBVL.BackEnd.WebApi.Modules.MasterData.Users.VerifiedUser;

public sealed class VerifiedUserEndpoint : IEndpoint
{
    public RouteHandlerBuilder RegisterTo(WebApplication app)
    {
        return app
            .MapPost(VerifiedUserRoute.Pattern, Handle)
            .AllowAnonymous()
            .WithTags(RouteConfig.Tag)
            .WithName(VerifiedUserRoute.Name)
            .WithDescription(VerifiedUserRoute.Description)
            .Produces<VerifiedUserResponse>();
    }

    private static async Task<IResult> Handle(
        [FromRoute] Guid userId,
        VerifiedUserCommand command,
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
