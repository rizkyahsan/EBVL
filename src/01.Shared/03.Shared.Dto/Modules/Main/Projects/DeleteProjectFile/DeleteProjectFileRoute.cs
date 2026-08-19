namespace EBVL.Shared.Dto.Modules.Main.Projects.DeleteProjectFile;

public static class DeleteProjectFileRoute
{
    public const string Name = $"{RouteConfig.Tag}.{nameof(DeleteProjectFile)}";
    public const string Description = $"{CommonDisplayTextFor.Delete} {ProjectsDisplayTextFor.Project} {CommonDisplayTextFor.File}";
    public const string Pattern = $"{RouteConfig.BasePath}/{{id:guid}}/{CommonDisplayTextFor.Delete}{CommonDisplayTextFor.File}";

    public static string ResourceUri(Guid id)
    {
        return $"{RouteConfig.BasePath}/{id}/{CommonDisplayTextFor.Delete}{CommonDisplayTextFor.File}";
    }
}
