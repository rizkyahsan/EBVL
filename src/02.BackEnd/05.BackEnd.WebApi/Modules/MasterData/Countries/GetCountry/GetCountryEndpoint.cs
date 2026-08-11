using EBVL.BackEnd.Logics.Modules.MasterData.Countries.GetCountry;
using EBVL.Shared.Dto.Modules.MasterData.Countries;
using EBVL.Shared.Dto.Modules.MasterData.Countries.GetCountry;

namespace EBVL.BackEnd.WebApi.Modules.MasterData.Countries.GetCountry;

public sealed class GetCountryEndpoint : IEndpoint
{
    public RouteHandlerBuilder RegisterTo(WebApplication app)
    {
        return app
            .MapGet(GetCountryRoute.Pattern, Handle)
            .RequireAuthorization()
            .WithTags(RouteConfig.Tag)
            .WithName(GetCountryRoute.Name)
            .WithDescription(GetCountryRoute.Description)
            .Produces<GetCountryResponse>();
    }

    private static async Task<IResult> Handle(
        [FromRoute] Guid countryId,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var query = new GetCountryQuery
        {
            CountryId = countryId
        };

        var response = await sender.Send(query, cancellationToken);

        return Results.Ok(response);
    }
}
