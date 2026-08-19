using EBVL.BackEnd.Logics.Modules.Authentication.ExternalUsers.SendOtpExternalUser;
using EBVL.Shared.Dto.Modules.Authentication.ExternalUsers.SendOtpExternalUser;
using EBVL.Shared.Dto.Modules.Authentication.ExternalUsers;

namespace EBVL.BackEnd.WebApi.Modules.Authentication.ExternalUsers.SendOtpExternalUser;

public sealed class SendOtpExternalUserEndpoint : IEndpoint
{
    public RouteHandlerBuilder RegisterTo(WebApplication app)
    {
        return app
            .MapPost(SendOtpExternalUserRoute.Pattern, Handle)
            .AllowAnonymous()
            .WithTags(RouteConfig.Tag)
            .WithName(SendOtpExternalUserRoute.Name)
            .WithDescription(SendOtpExternalUserRoute.Description)
            .Produces<SendOtpExternalUserResponse>();
    }

    private static async Task<IResult> Handle(
        [FromRoute] Guid id,
        SendOtpExternalUserCommand command,
        ISender sender,
        CancellationToken cancellationToken)
    {
        if (id != command.ExternalLoginId)
        {
            throw ExceptionFor.Mismatch(nameof(id), id, nameof(command.ExternalLoginId), command.ExternalLoginId);
        }

        var response = await sender.Send(command, cancellationToken);

        return Results.Ok(response);
    }
}
