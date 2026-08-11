using EBVL.BackEnd.Logics.Modules.MasterData.Countries.DeleteCountry;
using EBVL.Shared.Dto.Modules.MasterData.Countries;
using EBVL.Shared.Dto.Modules.MasterData.Countries.DeleteCountry;

namespace EBVL.BackEnd.WebApi.Modules.MasterData.Countries.DeleteCountry;

public sealed class DeleteCountryEndpoint : IEndpoint
{
    public RouteHandlerBuilder RegisterTo(WebApplication app)
    {
        return app
            .MapDelete(DeleteCountryRoute.Pattern, Handle)
            .RequireAuthorization()
            .WithTags(RouteConfig.Tag)
            .WithName(DeleteCountryRoute.Name)
            .WithDescription(DeleteCountryRoute.Description)
            .Produces(StatusCodes.Status204NoContent);
    }

    private static async Task<IResult> Handle(
        [FromRoute] Guid countryId,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new DeleteCountryCommand
        {
            CountryId = countryId
        };

        await sender.Send(command, cancellationToken);

        return Results.NoContent();
    }
}
