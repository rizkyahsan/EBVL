using EBVL.BackEnd.Logics.Modules.MasterData.Countries.GetCountries;
using EBVL.Shared.Dto.Modules.MasterData.Countries;
using EBVL.Shared.Dto.Modules.MasterData.Countries.GetCountries;

namespace EBVL.BackEnd.WebApi.Modules.MasterData.Countries.GetCountries;

public sealed class GetCountriesEndpoint : IEndpoint
{
    public RouteHandlerBuilder RegisterTo(WebApplication app)
    {
        return app
            .MapGet(GetCountriesRoute.Pattern, Handle)
            .RequireAuthorization()
            .WithTags(RouteConfig.Tag)
            .WithName(GetCountriesRoute.Name)
            .WithDescription(GetCountriesRoute.Description)
            .Produces<GetCountriesResponse>();
    }

    private static async Task<IResult> Handle(
        ISender sender,
        CancellationToken cancellationToken)
    {
        var query = new GetCountriesQuery();
        var response = await sender.Send(query, cancellationToken);

        return Results.Ok(response);
    }
}
