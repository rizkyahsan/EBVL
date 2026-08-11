namespace EBVL.Shared.Dto.Modules.MasterData.Countries.AddCountry;

public static class AddCountryRoute
{
    public const string Name = $"{RouteConfig.Tag}.{nameof(AddCountry)}";
    public const string Description = $"{CommonDisplayTextFor.Add} {CountriesDisplayTextFor.Country}";
    public const string Pattern = RouteConfig.BasePath;
    public const string ResourceUri = RouteConfig.BasePath;
}
