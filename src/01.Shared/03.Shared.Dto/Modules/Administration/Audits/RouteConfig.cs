namespace EBVL.Shared.Dto.Modules.Administration.Audits;

public static class RouteConfig
{
    public const string BasePath = $"/{ModuleConfig.Prefix}/{nameof(Audits)}";
    public const string Tag = $"{ModuleConfig.Prefix}.{nameof(Audits)}";
}
