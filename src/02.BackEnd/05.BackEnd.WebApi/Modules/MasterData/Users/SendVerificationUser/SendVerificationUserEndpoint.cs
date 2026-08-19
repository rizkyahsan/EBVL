using EBVL.BackEnd.Logics.Modules.MasterData.Users.SendVerificationUser;
using EBVL.Shared.Dto.Modules.MasterData.Users;
using EBVL.Shared.Dto.Modules.MasterData.Users.SendVerificationUser;

namespace EBVL.BackEnd.WebApi.Modules.MasterData.Users.SendVerificationUser;

public sealed class SendVerificationUserEndpoint : IEndpoint
{
    public RouteHandlerBuilder RegisterTo(WebApplication app)
    {
        return app
            .MapPost(SendVerificationUserRoute.Pattern, Handle)
            .RequireAuthorization()
            .WithTags(RouteConfig.Tag)
            .WithName(SendVerificationUserRoute.Name)
            .WithDescription(SendVerificationUserRoute.Description)
            .Produces<SendVerificationUserResponse>();
    }

    private static async Task<IResult> Handle(
        [FromRoute] Guid userId,
        SendVerificationUserCommand command,
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
