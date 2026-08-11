namespace EBVL.Shared.Dto.Modules.Examples.Documents.UpdateDocument;

public static class UpdateDocumentRoute
{
    public const string Name = $"{RouteConfig.Tag}.{nameof(UpdateDocument)}";
    public const string Description = $"{CommonDisplayTextFor.Update} {DocumentsDisplayTextFor.Document}";
    public const string Pattern = $"{RouteConfig.BasePath}/{{documentId:guid}}";

    public static string ResourceUri(Guid documentId)
    {
        return $"{RouteConfig.BasePath}/{documentId}";
    }
}
