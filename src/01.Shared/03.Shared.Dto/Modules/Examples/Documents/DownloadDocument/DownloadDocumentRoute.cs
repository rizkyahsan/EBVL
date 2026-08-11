namespace EBVL.Shared.Dto.Modules.Examples.Documents.DownloadDocument;

public static class DownloadDocumentRoute
{
    public const string Name = $"{RouteConfig.Tag}.{nameof(DownloadDocument)}";
    public const string Description = $"{CommonDisplayTextFor.Download} {DocumentsDisplayTextFor.Document}";
    public const string Pattern = $"{RouteConfig.BasePath}/{{documentId:guid}}/{CommonDisplayTextFor.Download}";

    public static string ResourceUri(Guid documentId)
    {
        return $"{RouteConfig.BasePath}/{documentId}/{CommonDisplayTextFor.Download}";
    }
}
