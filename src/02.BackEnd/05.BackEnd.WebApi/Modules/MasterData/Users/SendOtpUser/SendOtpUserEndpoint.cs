using EBVL.BackEnd.Logics.Modules.MasterData.Users.SendOtpUser;
using EBVL.Shared.Dto.Modules.MasterData.Users;
using EBVL.Shared.Dto.Modules.MasterData.Users.SendOtpUser;

namespace EBVL.BackEnd.WebApi.Modules.MasterData.Users.SendOtpUser;

public sealed class SendOtpUserEndpoint : IEndpoint
{
    public RouteHandlerBuilder RegisterTo(WebApplication app)
    {
        return app
            .MapPost(SendOtpUserRoute.Pattern, Handle)
            .AllowAnonymous()
            .WithTags(RouteConfig.Tag)
            .WithName(SendOtpUserRoute.Name)
            .WithDescription(SendOtpUserRoute.Description)
            .Produces<SendOtpUserResponse>();
    }

    private static async Task<IResult> Handle(
        [FromRoute] Guid userId,
        SendOtpUserCommand command,
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
