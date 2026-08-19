namespace EBVL.Shared.Dto.Modules.Common.FileStorages.DownloadFileStorage;

public static class DownloadFileStorageRoute
{
    public const string Name = $"{RouteConfig.Tag}.{nameof(DownloadFileStorage)}";
    public static readonly string Description = $"{CommonDisplayTextFor.Download} {FileStoragesDisplayTextFor.FileStorage}";
    public const string Pattern = $"{RouteConfig.BasePath}/{{fileStorageId:guid}}/{CommonDisplayTextFor.Download}";

    public static string ResourceUri(Guid fileStorageId)
    {
        return $"{RouteConfig.BasePath}/{fileStorageId}/{CommonDisplayTextFor.Download}";
    }
}
