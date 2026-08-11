namespace EBVL.Shared.Dto.Modules.Examples.Documents.GetDocument;

public static class GetDocumentRoute
{
    public const string Name = $"{RouteConfig.Tag}.{nameof(GetDocument)}";
    public const string Description = $"{CommonDisplayTextFor.Get} {DocumentsDisplayTextFor.Document}";
    public const string Pattern = $"{RouteConfig.BasePath}/{{documentId:guid}}";

    public static string ResourceUri(Guid documentId)
    {
        return $"{RouteConfig.BasePath}/{documentId}";
    }
}
