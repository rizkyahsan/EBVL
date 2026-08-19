namespace EBVL.Shared.Dto.Modules.Log.LogEmails;

public static class RouteConfig
{
    public const string BasePath = $"/{ModuleConfig.Prefix}/{nameof(LogEmails)}";
    public const string Tag = $"{ModuleConfig.Prefix}.{nameof(LogEmails)}";
}
