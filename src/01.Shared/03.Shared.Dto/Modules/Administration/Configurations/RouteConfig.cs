namespace EBVL.Shared.Dto.Modules.Administration.Configurations;

public static class RouteConfig
{
    public const string BasePath = $"/{ModuleConfig.Prefix}/{nameof(Configurations)}";
    public const string Tag = $"{ModuleConfig.Prefix}.{nameof(Configurations)}";
}
