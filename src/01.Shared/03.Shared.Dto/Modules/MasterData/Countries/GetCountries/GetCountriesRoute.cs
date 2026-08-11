namespace EBVL.Shared.Dto.Modules.MasterData.Countries.GetCountries;

public static class GetCountriesRoute
{
    public const string Name = $"{RouteConfig.Tag}.{nameof(GetCountries)}";
    public const string Description = $"{CommonDisplayTextFor.Get} {CountriesDisplayTextFor.Countries}";
    public const string Pattern = RouteConfig.BasePath;
    public const string ResourceUri = RouteConfig.BasePath;
}
