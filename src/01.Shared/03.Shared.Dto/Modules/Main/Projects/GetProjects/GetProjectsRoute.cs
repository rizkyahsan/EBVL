namespace EBVL.Shared.Dto.Modules.Main.Projects.GetProjects;

public static class GetProjectsRoute
{
    public const string Name = $"{RouteConfig.Tag}.{nameof(GetProjects)}";
    public const string Description = $"{CommonDisplayTextFor.Get} {ProjectsDisplayTextFor.Projects}";
    public const string Pattern = RouteConfig.BasePath;
    public const string ResourceUri = RouteConfig.BasePath;
}
