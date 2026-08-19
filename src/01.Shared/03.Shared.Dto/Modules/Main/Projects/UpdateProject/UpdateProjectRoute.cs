namespace EBVL.Shared.Dto.Modules.Main.Projects.UpdateProject;

public static class UpdateProjectRoute
{
    public const string Name = $"{RouteConfig.Tag}.{nameof(UpdateProject)}";
    public const string Description = $"{CommonDisplayTextFor.Update} {ProjectsDisplayTextFor.Project}";
    public const string Pattern = $"{RouteConfig.BasePath}/{{id:guid}}/{CommonDisplayTextFor.Update}";

    public static string ResourceUri(Guid id)
    {
        return $"{RouteConfig.BasePath}/{id}/{CommonDisplayTextFor.Update}";
    }
}
