namespace EBVL.Shared.Dto.Modules.Administration.ApiCalls;

public static class RouteConfig
{
    public const string BasePath = $"/{ModuleConfig.Prefix}/{nameof(ApiCalls)}";
    public const string Tag = $"{ModuleConfig.Prefix}.{nameof(ApiCalls)}";
}
