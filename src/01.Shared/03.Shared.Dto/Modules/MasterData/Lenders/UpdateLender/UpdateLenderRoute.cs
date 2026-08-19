namespace EBVL.Shared.Dto.Modules.MasterData.Lenders.UpdateLender;

public static class UpdateLenderRoute
{
    public const string Name = $"{RouteConfig.Tag}.{nameof(UpdateLender)}";
    public const string Description = $"{CommonDisplayTextFor.Update} {LendersDisplayTextFor.Lender}";
    public const string Pattern = $"{RouteConfig.BasePath}/{{lenderId:guid}}";

    public static string ResourceUri(Guid lenderId)
    {
        return $"{RouteConfig.BasePath}/{lenderId}";
    }
}
