namespace EBVL.Shared.Dto.Modules.Main.Projects.GetProject;

public static class GetProjectRoute
{
    public const string Name = $"{RouteConfig.Tag}.{nameof(GetProject)}";
    public const string Description = $"{CommonDisplayTextFor.Get} {ProjectsDisplayTextFor.Project}";
    public const string Pattern = $"{RouteConfig.BasePath}/{{id:guid}}";

    public static string ResourceUri(Guid id)
    {
        return $"{RouteConfig.BasePath}/{id}";
    }
}
