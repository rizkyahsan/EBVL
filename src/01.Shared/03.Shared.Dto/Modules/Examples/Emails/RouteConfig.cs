namespace EBVL.Shared.Dto.Modules.Examples.Emails;

public static class RouteConfig
{
    public const string BasePath = $"/{ModuleConfig.Prefix}/{nameof(Emails)}";
    public const string Tag = $"{ModuleConfig.Prefix}.{nameof(Emails)}";
}
