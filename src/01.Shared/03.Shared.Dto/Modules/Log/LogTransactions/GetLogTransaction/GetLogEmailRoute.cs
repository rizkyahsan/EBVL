namespace EBVL.Shared.Dto.Modules.Log.LogTransactions.GetLogTransaction;

public static class GetLogTransactionRoute
{
    public const string Name = $"{RouteConfig.Tag}.{nameof(GetLogTransaction)}";
    public static readonly string Description = $"{CommonDisplayTextFor.Get} {LogTransactionsDisplayTextFor.LogTransaction}";
    public const string Pattern = $"{RouteConfig.BasePath}/{{id:guid}}";

    public static string ResourceUri(Guid id)
    {
        return $"{RouteConfig.BasePath}/{id}";
    }
}
