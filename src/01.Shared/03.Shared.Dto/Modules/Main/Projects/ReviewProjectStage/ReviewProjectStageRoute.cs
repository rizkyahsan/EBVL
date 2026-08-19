namespace EBVL.Shared.Dto.Modules.Main.Projects.ReviewProjectStage;

public static class ReviewProjectStageRoute
{
    public const string Name = $"{RouteConfig.Tag}.{nameof(ReviewProjectStage)}";
    public static readonly string Description = $"Review {ProjectStagesDisplayTextFor.ProjectStage}";
    public static readonly string Pattern = $"{RouteConfig.BasePath}/ProjectStage/{{id:guid}}/Review";

    public static string ResourceUri(Guid id)
    {
        return $"{RouteConfig.BasePath}/ProjectStage/{id}/Review";
    }
}
