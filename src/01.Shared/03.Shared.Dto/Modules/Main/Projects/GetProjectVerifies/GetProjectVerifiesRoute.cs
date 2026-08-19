namespace EBVL.Shared.Dto.Modules.Main.Projects.GetProjectVerifies;

public static class GetProjectVerifiesRoute
{
    public const string Name = $"{RouteConfig.Tag}.{nameof(GetProjectVerifies)}";
    public const string Description = $"{CommonDisplayTextFor.Get} Project Verified";
    public const string Pattern = $"{RouteConfig.BasePath}/ProjectVerified";
    public const string ResourceUri = $"{RouteConfig.BasePath}/ProjectVerified";
}
