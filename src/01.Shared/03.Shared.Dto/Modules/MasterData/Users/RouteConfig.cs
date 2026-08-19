namespace EBVL.Shared.Dto.Modules.MasterData.Users;

public static class RouteConfig
{
    public const string BasePath = $"/{ModuleConfig.Prefix}/{nameof(Users)}";
    public const string Tag = $"{ModuleConfig.Prefix}.{nameof(Users)}";
}
