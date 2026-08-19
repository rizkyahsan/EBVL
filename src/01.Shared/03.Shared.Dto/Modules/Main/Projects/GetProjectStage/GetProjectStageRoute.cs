namespace EBVL.Shared.Dto.Modules.Main.Projects.GetProjectStage;

public static class GetProjectStageRoute
{
    public const string Name = $"{RouteConfig.Tag}.{nameof(GetProjectStage)}";
    public static readonly string Description = $"{CommonDisplayTextFor.Get} {ProjectStagesDisplayTextFor.ProjectStage}";
    public static readonly string Pattern = $"{RouteConfig.BasePath}/ProjectStage/{{id:guid}}";

    public static string ResourceUri(Guid id)
    {
        return $"{RouteConfig.BasePath}/ProjectStage/{id}";
    }
}
