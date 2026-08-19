namespace EBVL.Shared.Dto.Modules.Main.Projects.RevisionProjectLenderReq;

public static class RevisionProjectLenderReqRoute
{
    public const string Name = $"{RouteConfig.Tag}.{nameof(RevisionProjectLenderReq)}";
    public static readonly string Description = $"Revision {ProjectStagesDisplayTextFor.ProjectStage}";
    public const string Pattern = $"{RouteConfig.BasePath}/ProjectStage/{{id:guid}}/Revision";

    public static string ResourceUri(Guid id)
    {
        return $"{RouteConfig.BasePath}/ProjectStage/{id}/Revision";
    }
}
