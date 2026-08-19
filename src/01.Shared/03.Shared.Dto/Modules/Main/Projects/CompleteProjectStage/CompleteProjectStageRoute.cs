namespace EBVL.Shared.Dto.Modules.Main.Projects.CompleteProjectStage;

public static class CompleteProjectStageRoute
{
    public const string Name = $"{RouteConfig.Tag}.{nameof(CompleteProjectStage)}";
    public static readonly string Description = $"Complete {ProjectStagesDisplayTextFor.ProjectStage}";
    public static readonly string Pattern = $"{RouteConfig.BasePath}/ProjectStage/{{id:guid}}/Complete";

    public static string ResourceUri(Guid id)
    {
        return $"{RouteConfig.BasePath}/ProjectStage/{id}/Complete";
    }
}
