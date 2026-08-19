namespace EBVL.Shared.Dto.Modules.MasterData.Lenders.GetLenders;

public static class GetLendersRoute
{
    public const string Name = $"{RouteConfig.Tag}.{nameof(GetLenders)}";
    public const string Description = $"{CommonDisplayTextFor.Get} {LendersDisplayTextFor.Lenders}";
    public const string Pattern = RouteConfig.BasePath;
    public const string ResourceUri = RouteConfig.BasePath;
}
