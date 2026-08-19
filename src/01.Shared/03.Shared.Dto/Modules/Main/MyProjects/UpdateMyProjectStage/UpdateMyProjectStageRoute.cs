namespace EBVL.Shared.Dto.Modules.Main.MyProjects.UpdateMyProjectStage;

public static class UpdateMyProjectStageRoute
{
    public const string Name = $"{RouteConfig.Tag}.{nameof(UpdateMyProjectStage)}";
    public static readonly string Description = $"{CommonDisplayTextFor.Update} {ProjectStagesDisplayTextFor.MyProjectStage}";
    public static readonly string Pattern = $"{RouteConfig.BasePath}/MyProjectStage/{{id:guid}}/{CommonDisplayTextFor.Update}";

    public static string ResourceUri(Guid id)
    {
        return $"{RouteConfig.BasePath}/MyProjectStage/{id}/{CommonDisplayTextFor.Update}";
    }
}
