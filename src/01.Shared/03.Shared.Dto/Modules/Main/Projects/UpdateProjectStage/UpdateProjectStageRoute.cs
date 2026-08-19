namespace EBVL.Shared.Dto.Modules.Main.Projects.UpdateProjectStage;

public static class UpdateProjectStageRoute
{
    public const string Name = $"{RouteConfig.Tag}.{nameof(UpdateProjectStage)}";
    public static readonly string Description = $"{CommonDisplayTextFor.Update} {ProjectStagesDisplayTextFor.ProjectStage}";
    public const string Pattern = $"{RouteConfig.BasePath}/ProjectStage/{{id:guid}}/{CommonDisplayTextFor.Update}";

    public static string ResourceUri(Guid id)
    {
        return $"{RouteConfig.BasePath}/ProjectStage/{id}/{CommonDisplayTextFor.Update}";
    }
}
