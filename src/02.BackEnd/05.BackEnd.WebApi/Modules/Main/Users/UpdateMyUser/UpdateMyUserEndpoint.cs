using EBVL.BackEnd.Logics.Modules.Main.Users.UpdateMyUser;
using EBVL.Shared.Dto.Modules.Main.Users;
using EBVL.Shared.Dto.Modules.Main.Users.UpdateMyUser;

namespace EBVL.BackEnd.WebApi.Modules.Main.Users.UpdateMyUser;

public sealed class UpdateMyUserEndpoint : IEndpoint
{
    public RouteHandlerBuilder RegisterTo(WebApplication app)
    {
        return app
            .MapPatch(UpdateMyUserRoute.Pattern, Handle)
            .RequireAuthorization()
            .WithTags(RouteConfig.Tag)
            .WithName(UpdateMyUserRoute.Name)
            .WithDescription(UpdateMyUserRoute.Description)
            .Produces(StatusCodes.Status204NoContent);
    }

    private static async Task<IResult> Handle(
        UpdateMyUserCommand command,
        ISender sender,
        CancellationToken cancellationToken)
    {
        await sender.Send(command, cancellationToken);

        return Results.NoContent();
    }
}
