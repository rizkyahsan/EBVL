using EBVL.BackEnd.Logics.Modules.Authentication.ExternalUsers.VerifiedExternalUser;
using EBVL.Shared.Dto.Modules.Authentication.ExternalUsers;
using EBVL.Shared.Dto.Modules.Authentication.ExternalUsers.VerifiedExternalUser;

namespace EBVL.BackEnd.WebApi.Modules.Authentication.ExternalUsers.VerifiedExternalUser;

public sealed class VerifiedExternalUserEndpoint : IEndpoint
{
    public RouteHandlerBuilder RegisterTo(WebApplication app)
    {
        return app
            .MapPost(VerifiedExternalUserRoute.Pattern, Handle)
            .AllowAnonymous()
            .WithTags(RouteConfig.Tag)
            .WithName(VerifiedExternalUserRoute.Name)
            .WithDescription(VerifiedExternalUserRoute.Description)
            .Produces<VerifiedExternalUserResponse>();
    }

    private static async Task<IResult> Handle(
        [FromRoute] Guid id,
        VerifiedExternalUserCommand command,
        ISender sender,
        IHttpContextAccessor httpContextAccessor,
        CancellationToken cancellationToken)
    {
        if (id != command.ExternalLoginId)
        {
            throw ExceptionFor.Mismatch(nameof(id), id, nameof(command.ExternalLoginId), command.ExternalLoginId);
        }

        var response = await sender.Send(command, cancellationToken);

        if (!response.Item.Succeeded)
        {
            return Results.BadRequest(response);
        }

        return Results.Ok(response);
    }
}
