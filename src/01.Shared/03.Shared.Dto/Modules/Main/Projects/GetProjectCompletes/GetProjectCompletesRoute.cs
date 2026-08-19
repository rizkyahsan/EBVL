namespace EBVL.Shared.Dto.Modules.Main.Projects.GetProjectCompletes;

public static class GetProjectCompletesRoute
{
    public const string Name = $"{RouteConfig.Tag}.{nameof(GetProjectCompletes)}";
    public const string Description = $"{CommonDisplayTextFor.Get} Project Completed";
    public const string Pattern = $"{RouteConfig.BasePath}/ProjectCompleted";
    public const string ResourceUri = $"{RouteConfig.BasePath}/ProjectCompleted";
}
