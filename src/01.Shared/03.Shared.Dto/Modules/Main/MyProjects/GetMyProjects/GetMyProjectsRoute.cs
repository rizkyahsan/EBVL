namespace EBVL.Shared.Dto.Modules.Main.MyProjects.GetMyProjects;

public static class GetMyProjectsRoute
{
    public const string Name = $"{RouteConfig.Tag}.{nameof(GetMyProjects)}";
    public static readonly string Description = $"{CommonDisplayTextFor.Get} {ProjectsDisplayTextFor.MyProjects}";
    public const string Pattern = RouteConfig.BasePath;
    public const string ResourceUri = RouteConfig.BasePath;
}
