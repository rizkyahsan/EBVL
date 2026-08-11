namespace EBVL.Shared.Dto.Modules.Examples.Documents.AddDocument;

public static class AddDocumentRoute
{
    public const string Name = $"{RouteConfig.Tag}.{nameof(AddDocument)}";
    public const string Description = $"{CommonDisplayTextFor.Add} {DocumentsDisplayTextFor.Document}";
    public const string Pattern = RouteConfig.BasePath;
    public const string ResourceUri = RouteConfig.BasePath;
}
