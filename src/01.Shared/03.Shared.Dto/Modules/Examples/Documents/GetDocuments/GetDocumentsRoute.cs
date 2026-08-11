namespace EBVL.Shared.Dto.Modules.Examples.Documents.GetDocuments;

public static class GetDocumentsRoute
{
    public const string Name = $"{RouteConfig.Tag}.{nameof(GetDocuments)}";
    public const string Description = $"{CommonDisplayTextFor.Get} {DocumentsDisplayTextFor.Documents}";
    public const string Pattern = RouteConfig.BasePath;
    public const string ResourceUri = RouteConfig.BasePath;
}
