namespace EBVL.Shared.Dto.Modules.Authentication.ExternalUsers;

public static class RouteConfig
{
    public const string BasePath = $"/{ModuleConfig.Prefix}/{nameof(ExternalUsers)}";
    public const string Tag = $"{ModuleConfig.Prefix}.{nameof(ExternalUsers)}";
}
