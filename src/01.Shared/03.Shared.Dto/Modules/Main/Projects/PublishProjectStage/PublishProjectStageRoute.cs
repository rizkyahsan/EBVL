namespace EBVL.Shared.Dto.Modules.Main.Projects.PublishProjectStage;

public static class PublishProjectStageRoute
{
    public const string Name = $"{RouteConfig.Tag}.{nameof(PublishProjectStage)}";
    public static readonly string Description = $"{CommonDisplayTextFor.Publish} {ProjectStagesDisplayTextFor.ProjectStage}";
    public static readonly string Pattern = $"{RouteConfig.BasePath}/ProjectStage/{{id:guid}}/{CommonDisplayTextFor.Publish}";

    public static string ResourceUri(Guid id)
    {
        return $"{RouteConfig.BasePath}/ProjectStage/{id}/{CommonDisplayTextFor.Publish}";
    }
}
