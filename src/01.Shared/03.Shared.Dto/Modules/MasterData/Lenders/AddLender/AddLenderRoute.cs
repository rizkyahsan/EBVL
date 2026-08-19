namespace EBVL.Shared.Dto.Modules.MasterData.Lenders.AddLender;

public static class AddLenderRoute
{
    public const string Name = $"{RouteConfig.Tag}.{nameof(AddLender)}";
    public const string Description = $"{CommonDisplayTextFor.Add} {LendersDisplayTextFor.Lender}";
    public const string Pattern = RouteConfig.BasePath;
    public const string ResourceUri = RouteConfig.BasePath;
}
