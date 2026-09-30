namespace EBVL.Shared.Dto.Modules.MasterData.Documents;

public static class GetDocumentsRoute { public const string Pattern = RouteConfig.BasePath; }
public static class GetDocumentRoute
{
    public const string Pattern = RouteConfig.BasePath + "/{documentRequirementSetId:guid}"; public static string ResourceUri(Guid id)
    {
        return $"{RouteConfig.BasePath}/{id}";
    }
}
public static class GetDocumentHistoryRoute
{
    public const string Pattern = RouteConfig.BasePath + "/series/{seriesId:guid}/versions"; public static string ResourceUri(Guid id)
    {
        return $"{RouteConfig.BasePath}/series/{id}/versions";
    }
}
public static class CreateDocumentDraftRoute
{
    public const string Pattern = GetDocumentRoute.Pattern + "/draft"; public static string ResourceUri(Guid id)
    {
        return $"{GetDocumentRoute.ResourceUri(id)}/draft";
    }
}
public static class PublishDocumentRequirementSetRoute
{
    public const string Pattern = GetDocumentRoute.Pattern + "/publish"; public static string ResourceUri(Guid id)
    {
        return $"{GetDocumentRoute.ResourceUri(id)}/publish";
    }
}
public static class AddDocumentRoute
{
    public const string Pattern = GetDocumentRoute.Pattern + "/requirements"; public static string ResourceUri(Guid id)
    {
        return $"{GetDocumentRoute.ResourceUri(id)}/requirements";
    }
}
public static class UpdateDocumentRoute
{
    public const string Pattern = AddDocumentRoute.Pattern + "/{documentId:guid}"; public static string ResourceUri(Guid setId, Guid documentId)
    {
        return $"{AddDocumentRoute.ResourceUri(setId)}/{documentId}";
    }
}
public static class DeleteDocumentRoute
{
    public const string Pattern = UpdateDocumentRoute.Pattern; public static string ResourceUri(Guid setId, Guid documentId)
    {
        return UpdateDocumentRoute.ResourceUri(setId, documentId);
    }
}
