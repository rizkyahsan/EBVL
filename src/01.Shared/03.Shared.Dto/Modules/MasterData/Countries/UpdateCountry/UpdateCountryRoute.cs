namespace EBVL.Shared.Dto.Modules.MasterData.Countries.UpdateCountry;

public static class UpdateCountryRoute
{
    public const string Name = $"{RouteConfig.Tag}.{nameof(UpdateCountry)}";
    public const string Description = $"{CommonDisplayTextFor.Update} {CountriesDisplayTextFor.Country}";
    public const string Pattern = $"{RouteConfig.BasePath}/{{countryId:guid}}";

    public static string ResourceUri(Guid countryId)
    {
        return $"{RouteConfig.BasePath}/{countryId}";
    }
}
