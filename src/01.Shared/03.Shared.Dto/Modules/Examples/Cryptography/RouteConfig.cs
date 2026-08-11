namespace EBVL.Shared.Dto.Modules.Examples.Cryptography;

public static class RouteConfig
{
    public const string BasePath = $"/{ModuleConfig.Prefix}/{nameof(Cryptography)}";
    public const string Tag = $"{ModuleConfig.Prefix}.{nameof(Cryptography)}";
}
