namespace EBVL.Shared.Dto.Modules.Administration.Configurations.GetConfigurations;

public static class GetConfigurationsRoute
{
    public const string Name = $"{RouteConfig.Tag}.{nameof(GetConfigurations)}";
    public const string Description = $"{CommonDisplayTextFor.Get} {ConfigurationsDisplayTextFor.Configurations}";
    public const string Pattern = RouteConfig.BasePath;
    public const string ResourceUri = RouteConfig.BasePath;
}
