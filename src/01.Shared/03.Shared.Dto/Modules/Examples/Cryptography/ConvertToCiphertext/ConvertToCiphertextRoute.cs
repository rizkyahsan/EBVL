namespace EBVL.Shared.Dto.Modules.Examples.Cryptography.ConvertToCiphertext;

public static class ConvertToCiphertextRoute
{
    public const string Name = $"{RouteConfig.Tag}.{nameof(ConvertToCiphertext)}";
    public const string Description = $"{CommonDisplayTextFor.Convert} to {CommonDisplayTextFor.Ciphertext}";
    public const string Pattern = $"{RouteConfig.BasePath}/{nameof(ConvertToCiphertext)}";
    public const string ResourceUri = $"{RouteConfig.BasePath}/{nameof(ConvertToCiphertext)}";
}
