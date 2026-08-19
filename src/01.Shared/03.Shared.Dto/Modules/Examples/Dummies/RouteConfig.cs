namespace EBVL.Shared.Dto.Modules.Examples.Dummies;

public static class RouteConfig
{
    public const string BasePath = $"/{ModuleConfig.Prefix}/{nameof(Dummies)}";
    public const string Tag = $"{ModuleConfig.Prefix}.{nameof(Dummies)}";
}
