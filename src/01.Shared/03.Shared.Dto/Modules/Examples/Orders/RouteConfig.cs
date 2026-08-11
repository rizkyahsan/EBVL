namespace EBVL.Shared.Dto.Modules.Examples.Orders;

public static class RouteConfig
{
    public const string BasePath = $"/{ModuleConfig.Prefix}/{nameof(Orders)}";
    public const string Tag = $"{ModuleConfig.Prefix}.{nameof(Orders)}";
}
