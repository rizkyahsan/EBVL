using EBVL.BackEnd.Logics.Modules.Authentication.ExternalUsers.LoginExternalUser;
using EBVL.Shared.Dto.Modules.Authentication.ExternalUsers;
using EBVL.Shared.Dto.Modules.Authentication.ExternalUsers.LoginExternalUser;

namespace EBVL.BackEnd.WebApi.Modules.Authentication.ExternalUsers.LoginExternalUser;

public sealed class LoginExternalUserEndpoint : IEndpoint
{
    public RouteHandlerBuilder RegisterTo(WebApplication app)
    {
        return app
            .MapPost(LoginExternalUserRoute.Pattern, Handle)
            .AllowAnonymous()
            .WithTags(RouteConfig.Tag)
            .WithName(LoginExternalUserRoute.Name)
            .WithDescription(LoginExternalUserRoute.Description)
            .Produces<LoginExternalUserResponse>();
    }

    private static async Task<IResult> Handle(
        LoginExternalUserCommand command,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var response = await sender.Send(command, cancellationToken);

        return Results.Ok(response);
    }
}
