using EBVL.BackEnd.Logics.Modules.Administration.Configurations.AddConfiguration;
using EBVL.Shared.Dto.Modules.Administration.Configurations;
using EBVL.Shared.Dto.Modules.Administration.Configurations.AddConfiguration;
using EBVL.Shared.Dto.Modules.Administration.Configurations.GetConfiguration;

namespace EBVL.BackEnd.WebApi.Modules.Administration.Configurations.AddConfiguration;

public sealed class AddConfigurationEndpoint : IEndpoint
{
    public RouteHandlerBuilder RegisterTo(WebApplication app)
    {
        return app
            .MapPost(AddConfigurationRoute.Pattern, Handle)
            .RequireAuthorization()
            .WithTags(RouteConfig.Tag)
            .WithName(AddConfigurationRoute.Name)
            .WithDescription(AddConfigurationRoute.Description)
            .Produces<AddConfigurationResponse>(StatusCodes.Status201Created);
    }

    private static async Task<IResult> Handle(
        AddConfigurationCommand command,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var response = await sender.Send(command, cancellationToken);

        return Results.Created(GetConfigurationRoute.ResourceUri(response.Item.Id), response);
    }
}
