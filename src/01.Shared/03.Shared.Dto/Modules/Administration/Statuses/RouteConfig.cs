namespace EBVL.Shared.Dto.Modules.Administration.Statuses;

public static class RouteConfig
{
    public const string BasePath = $"/{ModuleConfig.Prefix}/{nameof(Statuses)}";
    public const string Tag = $"{ModuleConfig.Prefix}.{nameof(Statuses)}";
}
