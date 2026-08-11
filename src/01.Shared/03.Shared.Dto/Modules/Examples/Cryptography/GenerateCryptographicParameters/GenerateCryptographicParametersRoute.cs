namespace EBVL.Shared.Dto.Modules.Examples.Cryptography.GenerateCryptographicParameters;

public static class GenerateCryptographicParametersRoute
{
    public const string Name = $"{RouteConfig.Tag}.{nameof(GenerateCryptographicParameters)}";
    public const string Description = $"{CommonDisplayTextFor.Generate} {CommonDisplayTextFor.Cryptographic} {CommonDisplayTextFor.Parameters}";
    public const string Pattern = $"{RouteConfig.BasePath}/{nameof(GenerateCryptographicParameters)}";
    public const string ResourceUri = $"{RouteConfig.BasePath}/{nameof(GenerateCryptographicParameters)}";
}
