namespace EBVL.Shared.Dto.Modules.Main.Projects.CreateProjectStage;

public static class CreateProjectStageRoute
{
    public const string Name = $"{RouteConfig.Tag}.{nameof(CreateProjectStage)}";
    public static readonly string Description = $"{CommonDisplayTextFor.Create} {ProjectStagesDisplayTextFor.ProjectStage}";
    public const string Pattern = $"{RouteConfig.BasePath}/ProjectStage/{{id:guid}}/{CommonDisplayTextFor.Create}";

    public static string ResourceUri(Guid id)
    {
        return $"{RouteConfig.BasePath}/ProjectStage/{id}/{CommonDisplayTextFor.Create}";
    }
}
