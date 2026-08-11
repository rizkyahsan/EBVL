namespace EBVL.Shared.Dto.Modules.Examples.Orders.GetOrders;

public static class GetOrdersRoute
{
    public const string Name = $"{RouteConfig.Tag}.{nameof(GetOrders)}";
    public const string Description = $"{CommonDisplayTextFor.Get} Orders";
    public const string Pattern = RouteConfig.BasePath;
    public const string ResourceUri = RouteConfig.BasePath;
}
