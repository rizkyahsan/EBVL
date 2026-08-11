using EBVL.BackEnd.Logics.Modules.Main.Users.SendMyVerificationCode;
using EBVL.Shared.Dto.Modules.Main.Users;
using EBVL.Shared.Dto.Modules.Main.Users.SendMyVerificationCode;

namespace EBVL.BackEnd.WebApi.Modules.Main.Users.SendMyVerificationCode;

public sealed class SendMyVerificationCodeEndpoint : IEndpoint
{
    public RouteHandlerBuilder RegisterTo(WebApplication app)
    {
        return app
            .MapPost(SendMyVerificationCodeRoute.Pattern, Handle)
            .RequireAuthorization()
            .WithTags(RouteConfig.Tag)
            .WithName(SendMyVerificationCodeRoute.Name)
            .WithDescription(SendMyVerificationCodeRoute.Description)
            .Produces<SendMyVerificationCodeResponse>();
    }

    private static async Task<IResult> Handle(
        ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new SendMyVerificationCodeCommand();
        var response = await sender.Send(command, cancellationToken);

        return Results.Ok(response);
    }
}
