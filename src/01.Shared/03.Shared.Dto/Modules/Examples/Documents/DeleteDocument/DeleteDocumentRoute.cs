namespace EBVL.Shared.Dto.Modules.Examples.Documents.DeleteDocument;

public static class DeleteDocumentRoute
{
    public const string Name = $"{RouteConfig.Tag}.{nameof(DeleteDocument)}";
    public const string Description = $"{CommonDisplayTextFor.Delete} {DocumentsDisplayTextFor.Document}";
    public const string Pattern = $"{RouteConfig.BasePath}/{{documentId:guid}}";

    public static string ResourceUri(Guid documentId)
    {
        return $"{RouteConfig.BasePath}/{documentId}";
    }
}
