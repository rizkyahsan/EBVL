namespace EBVL.Shared.Dto.Modules.MasterData.Lenders;

public static class RouteConfig
{
    public const string BasePath = $"/{ModuleConfig.Prefix}/{nameof(Lenders)}";
    public const string Tag = $"{ModuleConfig.Prefix}.{nameof(Lenders)}";
}
