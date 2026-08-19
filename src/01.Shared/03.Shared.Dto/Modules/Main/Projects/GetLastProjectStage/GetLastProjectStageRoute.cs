namespace EBVL.Shared.Dto.Modules.Main.Projects.GetLastProjectStage;

public static class GetLastProjectStageRoute
{
    public const string Name = $"{RouteConfig.Tag}.Last{nameof(GetProjectStage)}";
    public static readonly string Description = $"{CommonDisplayTextFor.Get} Last {ProjectStagesDisplayTextFor.ProjectStage}";
    public static readonly string Pattern = $"{RouteConfig.BasePath}/ProjectStage/{{id:guid}}/Last";

    public static string ResourceUri(Guid id)
    {
        return $"{RouteConfig.BasePath}/ProjectStage/{id}/Last";
    }
}
