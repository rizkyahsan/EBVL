namespace EBVL.Shared.Dto.Modules.Main.Projects.GetProjectVerify;

public static class GetProjectVerifyRoute
{
    public const string Name = $"{RouteConfig.Tag}.{nameof(GetProjectVerify)}";
    public const string Description = $"{CommonDisplayTextFor.Get} Project Verified";
    public const string Pattern = $"{RouteConfig.BasePath}/ProjectVerified/{{id:guid}}";

    public static string ResourceUri(Guid id)
    {
        return $"{RouteConfig.BasePath}/ProjectVerified/{id}";
    }
}
