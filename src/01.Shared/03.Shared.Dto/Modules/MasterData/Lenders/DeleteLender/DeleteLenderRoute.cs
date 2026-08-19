namespace EBVL.Shared.Dto.Modules.MasterData.Lenders.DeleteLender;

public static class DeleteLenderRoute
{
    public const string Name = $"{RouteConfig.Tag}.{nameof(DeleteLender)}";
    public const string Description = $"{CommonDisplayTextFor.Delete} {LendersDisplayTextFor.Lender}";
    public const string Pattern = $"{RouteConfig.BasePath}/{{lenderId:guid}}";

    public static string ResourceUri(Guid lenderId)
    {
        return $"{RouteConfig.BasePath}/{lenderId}";
    }
}
