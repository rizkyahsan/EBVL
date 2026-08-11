using EBVL.BackEnd.Logics.Modules.MasterData.Countries.UpdateCountry;
using EBVL.Shared.Dto.Modules.MasterData.Countries;
using EBVL.Shared.Dto.Modules.MasterData.Countries.UpdateCountry;

namespace EBVL.BackEnd.WebApi.Modules.MasterData.Countries.UpdateCountry;

public sealed class UpdateCountryEndpoint : IEndpoint
{
    public RouteHandlerBuilder RegisterTo(WebApplication app)
    {
        return app
            .MapPatch(UpdateCountryRoute.Pattern, Handle)
            .RequireAuthorization()
            .WithTags(RouteConfig.Tag)
            .WithName(UpdateCountryRoute.Name)
            .WithDescription(UpdateCountryRoute.Description)
            .Produces(StatusCodes.Status204NoContent);
    }

    private static async Task<IResult> Handle(
        [FromRoute] Guid countryId,
        UpdateCountryCommand command,
        ISender sender,
        CancellationToken cancellationToken)
    {
        if (countryId != command.CountryId)
        {
            throw ExceptionFor.Mismatch(nameof(countryId), countryId, nameof(command.CountryId), command.CountryId);
        }

        await sender.Send(command, cancellationToken);

        return Results.NoContent();
    }
}
