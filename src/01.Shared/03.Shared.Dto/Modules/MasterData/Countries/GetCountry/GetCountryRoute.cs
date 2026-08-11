namespace EBVL.Shared.Dto.Modules.MasterData.Countries.GetCountry;

public static class GetCountryRoute
{
    public const string Name = $"{RouteConfig.Tag}.{nameof(GetCountry)}";
    public const string Description = $"{CommonDisplayTextFor.Get} {CountriesDisplayTextFor.Country}";
    public const string Pattern = $"{RouteConfig.BasePath}/{{countryId:guid}}";

    public static string ResourceUri(Guid countryId)
    {
        return $"{RouteConfig.BasePath}/{countryId}";
    }
}
