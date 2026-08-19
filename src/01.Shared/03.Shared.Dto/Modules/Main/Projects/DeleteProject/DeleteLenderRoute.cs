namespace EBVL.Shared.Dto.Modules.Main.Projects.DeleteProject;

public static class DeleteProjectRoute
{
    public const string Name = $"{RouteConfig.Tag}.{nameof(DeleteProject)}";
    public const string Description = $"{CommonDisplayTextFor.Delete} {ProjectsDisplayTextFor.Project}";
    public const string Pattern = $"{RouteConfig.BasePath}/{{projectId:guid}}/{CommonDisplayTextFor.Delete}";

    public static string ResourceUri(Guid projectId)
    {
        return $"{RouteConfig.BasePath}/{projectId}/{CommonDisplayTextFor.Delete}";
    }
}
