namespace EBVL.Shared.Dto.Modules.Main.Projects.UploadProjectFile;

public static class UploadProjectFileRoute
{
    public const string Name = $"{RouteConfig.Tag}.{nameof(UploadProjectFile)}";
    public const string Description = $"{CommonDisplayTextFor.Upload} {ProjectsDisplayTextFor.Project} {CommonDisplayTextFor.File}";
    public const string Pattern = $"{RouteConfig.BasePath}/{{id:guid}}/{CommonDisplayTextFor.Upload}";

    public static string ResourceUri(Guid id)
    {
        return $"{RouteConfig.BasePath}/{id}/{CommonDisplayTextFor.Upload}";
    }
}
