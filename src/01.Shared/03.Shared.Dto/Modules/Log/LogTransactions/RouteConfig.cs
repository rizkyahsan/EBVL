namespace EBVL.Shared.Dto.Modules.Log.LogTransactions;

public static class RouteConfig
{
    public const string BasePath = $"/{ModuleConfig.Prefix}/{nameof(LogTransactions)}";
    public const string Tag = $"{ModuleConfig.Prefix}.{nameof(LogTransactions)}";
}
