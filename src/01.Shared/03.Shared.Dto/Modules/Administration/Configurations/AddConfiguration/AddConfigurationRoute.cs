namespace EBVL.Shared.Dto.Modules.Administration.Configurations.AddConfiguration;

public static class AddConfigurationRoute
{
    public const string Name = $"{RouteConfig.Tag}.{nameof(AddConfiguration)}";
    public const string Description = $"{CommonDisplayTextFor.Add} {ConfigurationsDisplayTextFor.Configuration}";
    public const string Pattern = RouteConfig.BasePath;
    public const string ResourceUri = RouteConfig.BasePath;
}
