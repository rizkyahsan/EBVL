using EBVL.BackEnd.Logics.Modules.Authentication.ExternalUsers.CheckExternalUser;
using EBVL.Shared.Dto.Modules.Authentication.ExternalUsers;
using EBVL.Shared.Dto.Modules.Authentication.ExternalUsers.CheckExternalUser;

namespace EBVL.BackEnd.WebApi.Modules.Authentication.ExternalUsers.CheckExternalUser;

public sealed class CheckExternalUserEndpoint : IEndpoint
{
    public RouteHandlerBuilder RegisterTo(WebApplication app)
    {
        return app
            .MapGet(CheckExternalUserRoute.Pattern, Handle)
            .AllowAnonymous()
            .WithTags(RouteConfig.Tag)
            .WithName(CheckExternalUserRoute.Name)
            .WithDescription(CheckExternalUserRoute.Description)
            .Produces<CheckExternalUserResponse>();
    }

    private static async Task<IResult> Handle(
        [FromRoute] Guid id,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var query = new CheckExternalUserQuery
        {
            ExternalLoginId = id
        };

        var response = await sender.Send(query, cancellationToken);

        return Results.Ok(response);
    }
}

