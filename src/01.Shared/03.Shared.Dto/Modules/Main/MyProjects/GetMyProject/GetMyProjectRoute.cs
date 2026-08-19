namespace EBVL.Shared.Dto.Modules.Main.MyProjects.GetMyProject;

public static class GetMyProjectRoute
{
    public const string Name = $"{RouteConfig.Tag}.{nameof(GetMyProject)}";
    public static readonly string Description = $"{CommonDisplayTextFor.Get} {ProjectsDisplayTextFor.MyProject}";
    public const string Pattern = $"{RouteConfig.BasePath}/{{id:guid}}";

    public static string ResourceUri(Guid id)
    {
        return $"{RouteConfig.BasePath}/{id}";
    }
}
