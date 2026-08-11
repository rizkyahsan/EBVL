namespace EBVL.Shared.Dto.Modules.Administration.Configurations.GetConfiguration;

public static class GetConfigurationRoute
{
    public const string Name = $"{RouteConfig.Tag}.{nameof(GetConfiguration)}";
    public const string Description = $"{CommonDisplayTextFor.Get} {ConfigurationsDisplayTextFor.Configuration}";
    public const string Pattern = $"{RouteConfig.BasePath}/{{configurationId:guid}}";

    public static string ResourceUri(Guid configurationId)
    {
        return $"{RouteConfig.BasePath}/{configurationId}";
    }
}
