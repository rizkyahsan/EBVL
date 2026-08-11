namespace EBVL.Shared.Dto.Modules.Administration.Configurations.UpdateConfiguration;

public static class UpdateConfigurationRoute
{
    public const string Name = $"{RouteConfig.Tag}.{nameof(UpdateConfiguration)}";
    public const string Description = $"{CommonDisplayTextFor.Update} {ConfigurationsDisplayTextFor.Configuration}";
    public const string Pattern = $"{RouteConfig.BasePath}/{{configurationId:guid}}";

    public static string ResourceUri(Guid configurationId)
    {
        return $"{RouteConfig.BasePath}/{configurationId}";
    }
}
