using EBVL.BackEnd.Logics.Modules.MasterData.Users.ForgotPasswordUser;
using EBVL.Shared.Dto.Modules.MasterData.Users;
using EBVL.Shared.Dto.Modules.MasterData.Users.ForgotPasswordUser;

namespace EBVL.BackEnd.WebApi.Modules.MasterData.Users.ForgotPasswordUser;

public sealed class ForgotPasswordUserEndpoint : IEndpoint
{
    public RouteHandlerBuilder RegisterTo(WebApplication app)
    {
        return app.MapPost(ForgotPasswordUserRoute.Pattern, Handle)
            .AllowAnonymous()
            .WithTags(RouteConfig.Tag)
            .WithName(ForgotPasswordUserRoute.Name)
            .WithDescription(ForgotPasswordUserRoute.Description)
            .Produces<ForgotPasswordUserResponse>();
    }

    private static async Task<IResult> Handle(ForgotPasswordUserCommand command, ISender sender, CancellationToken cancellationToken)
    {
        return Results.Ok(await sender.Send(command, cancellationToken));
    }
}
