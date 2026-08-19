namespace EBVL.Shared.Dto.Modules.Log.LogTransactions.GetLogTransactions;

public static class GetLogTransactionsRoute
{
    public const string Name = $"{RouteConfig.Tag}.{nameof(GetLogTransactions)}";
    public static readonly string Description = $"{CommonDisplayTextFor.Get} {LogTransactionsDisplayTextFor.LogTransactions}";
    public const string Pattern = RouteConfig.BasePath;
    public const string ResourceUri = RouteConfig.BasePath;
}
