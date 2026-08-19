namespace EBVL.Shared.Dto.Modules.MasterData.Lenders.GetLender;

public static class GetLenderRoute
{
    public const string Name = $"{RouteConfig.Tag}.{nameof(GetLender)}";
    public const string Description = $"{CommonDisplayTextFor.Get} {LendersDisplayTextFor.Lender}";
    public const string Pattern = $"{RouteConfig.BasePath}/{{lenderId:guid}}";

    public static string ResourceUri(Guid lenderId)
    {
        return $"{RouteConfig.BasePath}/{lenderId}";
    }
}
