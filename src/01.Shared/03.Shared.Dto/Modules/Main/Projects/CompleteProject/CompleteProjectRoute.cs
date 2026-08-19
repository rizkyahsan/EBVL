namespace EBVL.Shared.Dto.Modules.Main.Projects.CompleteProject;

public static class CompleteProjectRoute
{
    public const string Name = $"{RouteConfig.Tag}.{nameof(CompleteProject)}";
    public const string Description = $"Complete {ProjectsDisplayTextFor.Project}";
    public const string Pattern = $"{RouteConfig.BasePath}/{{id:guid}}/Complete";

    public static string ResourceUri(Guid id)
    {
        return $"{RouteConfig.BasePath}/{id}/Complete";
    }
}
