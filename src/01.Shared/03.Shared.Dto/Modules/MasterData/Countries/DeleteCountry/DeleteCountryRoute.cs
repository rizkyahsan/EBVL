namespace EBVL.Shared.Dto.Modules.MasterData.Countries.DeleteCountry;

public static class DeleteCountryRoute
{
    public const string Name = $"{RouteConfig.Tag}.{nameof(DeleteCountry)}";
    public const string Description = $"{CommonDisplayTextFor.Delete} {CountriesDisplayTextFor.Country}";
    public const string Pattern = $"{RouteConfig.BasePath}/{{countryId:guid}}";

    public static string ResourceUri(Guid countryId)
    {
        return $"{RouteConfig.BasePath}/{countryId}";
    }
}
