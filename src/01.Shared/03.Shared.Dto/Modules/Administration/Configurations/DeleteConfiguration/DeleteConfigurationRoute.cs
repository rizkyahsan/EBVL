namespace EBVL.Shared.Dto.Modules.Administration.Configurations.DeleteConfiguration;

public static class DeleteConfigurationRoute
{
    public const string Name = $"{RouteConfig.Tag}.{nameof(DeleteConfiguration)}";
    public const string Description = $"{CommonDisplayTextFor.Delete} {ConfigurationsDisplayTextFor.Configuration}";
    public const string Pattern = $"{RouteConfig.BasePath}/{{configurationId:guid}}";

    public static string ResourceUri(Guid configurationId)
    {
        return $"{RouteConfig.BasePath}/{configurationId}";
    }
}
