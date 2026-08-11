using EBVL.BackEnd.Logics.Modules.Administration.Configurations.UpdateConfiguration;
using EBVL.Shared.Dto.Modules.Administration.Configurations;
using EBVL.Shared.Dto.Modules.Administration.Configurations.UpdateConfiguration;

namespace EBVL.BackEnd.WebApi.Modules.Administration.Configurations.UpdateConfiguration;

public sealed class UpdateConfigurationEndpoint : IEndpoint
{
    public RouteHandlerBuilder RegisterTo(WebApplication app)
    {
        return app
            .MapPatch(UpdateConfigurationRoute.Pattern, Handle)
            .RequireAuthorization()
            .WithTags(RouteConfig.Tag)
            .WithName(UpdateConfigurationRoute.Name)
            .WithDescription(UpdateConfigurationRoute.Description)
            .Produces(StatusCodes.Status204NoContent);
    }

    private static async Task<IResult> Handle(
        [FromRoute] Guid configurationId,
        UpdateConfigurationCommand command,
        ISender sender,
        CancellationToken cancellationToken)
    {
        if (configurationId != command.ConfigurationId)
        {
            throw ExceptionFor.Mismatch(nameof(configurationId), configurationId, nameof(command.ConfigurationId), command.ConfigurationId);
        }

        await sender.Send(command, cancellationToken);

        return Results.NoContent();
    }
}
