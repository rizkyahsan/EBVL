using EBVL.BackEnd.Logics.Modules.Administration.Configurations.GetConfigurations;
using EBVL.Shared.Dto.Modules.Administration.Configurations;
using EBVL.Shared.Dto.Modules.Administration.Configurations.GetConfigurations;

namespace EBVL.BackEnd.WebApi.Modules.Administration.Configurations.GetConfigurations;

public sealed class GetConfigurationsEndpoint : IEndpoint
{
    public RouteHandlerBuilder RegisterTo(WebApplication app)
    {
        return app
            .MapGet(GetConfigurationsRoute.Pattern, Handle)
            .RequireAuthorization()
            .WithTags(RouteConfig.Tag)
            .WithName(GetConfigurationsRoute.Name)
            .WithDescription(GetConfigurationsRoute.Description)
            .Produces<GetConfigurationsResponse>();
    }

    private static async Task<IResult> Handle(
        ISender sender,
        CancellationToken cancellationToken)
    {
        var query = new GetConfigurationsQuery();
        var response = await sender.Send(query, cancellationToken);

        return Results.Ok(response);
    }
}
