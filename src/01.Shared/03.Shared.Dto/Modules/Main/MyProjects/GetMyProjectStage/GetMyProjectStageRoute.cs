namespace EBVL.Shared.Dto.Modules.Main.MyProjects.GetMyProjectStage;

public static class GetMyProjectStageRoute
{
    public const string Name = $"{RouteConfig.Tag}.{nameof(GetMyProjectStage)}";
    public static readonly string Description = $"{CommonDisplayTextFor.Get} {ProjectStagesDisplayTextFor.MyProjectStage}";
    public static readonly string Pattern = $"{RouteConfig.BasePath}/MyProjectStage/{{id:guid}}";

    public static string ResourceUri(Guid id)
    {
        return $"{RouteConfig.BasePath}/MyProjectStage/{id}";
    }
}
