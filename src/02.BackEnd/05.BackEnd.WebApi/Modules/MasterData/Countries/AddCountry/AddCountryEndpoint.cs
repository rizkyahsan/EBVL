using EBVL.BackEnd.Logics.Modules.MasterData.Countries.AddCountry;
using EBVL.Shared.Dto.Modules.MasterData.Countries;
using EBVL.Shared.Dto.Modules.MasterData.Countries.AddCountry;
using EBVL.Shared.Dto.Modules.MasterData.Countries.GetCountry;

namespace EBVL.BackEnd.WebApi.Modules.MasterData.Countries.AddCountry;

public sealed class AddCountryEndpoint : IEndpoint
{
    public RouteHandlerBuilder RegisterTo(WebApplication app)
    {
        return app
            .MapPost(AddCountryRoute.Pattern, Handle)
            .RequireAuthorization()
            .WithTags(RouteConfig.Tag)
            .WithName(AddCountryRoute.Name)
            .WithDescription(AddCountryRoute.Description)
            .Produces<AddCountryResponse>(StatusCodes.Status201Created);
    }

    private static async Task<IResult> Handle(
        AddCountryCommand command,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var response = await sender.Send(command, cancellationToken);

        return Results.Created(GetCountryRoute.ResourceUri(response.Item.Id), response);
    }
}
