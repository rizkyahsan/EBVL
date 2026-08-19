namespace EBVL.Shared.Dto.Modules.Main.Projects.CreateProject;

public static class CreateProjectRoute
{
    public const string Name = $"{RouteConfig.Tag}.{nameof(CreateProject)}";
    public const string Description = $"{CommonDisplayTextFor.Create} {ProjectsDisplayTextFor.Project}";
    public const string Pattern = $"{RouteConfig.BasePath}/{CommonDisplayTextFor.Create}";
    public const string ResourceUri = $"{RouteConfig.BasePath}/{CommonDisplayTextFor.Create}";
}
