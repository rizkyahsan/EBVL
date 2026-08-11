namespace EBVL.Shared.Dto.Modules.MasterData.Countries;

public static class RouteConfig
{
    public const string BasePath = $"/{ModuleConfig.Prefix}/{nameof(Countries)}";
    public const string Tag = $"{ModuleConfig.Prefix}.{nameof(Countries)}";
}
