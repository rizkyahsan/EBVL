namespace EBVL.Shared.Dto.Modules.Examples.Cryptography.ConvertToPlaintext;

public static class ConvertToPlaintextRoute
{
    public const string Name = $"{RouteConfig.Tag}.{nameof(ConvertToPlaintext)}";
    public const string Description = $"{CommonDisplayTextFor.Convert} to {CommonDisplayTextFor.Plaintext}";
    public const string Pattern = $"{RouteConfig.BasePath}/{nameof(ConvertToPlaintext)}";
    public const string ResourceUri = $"{RouteConfig.BasePath}/{nameof(ConvertToPlaintext)}";
}
