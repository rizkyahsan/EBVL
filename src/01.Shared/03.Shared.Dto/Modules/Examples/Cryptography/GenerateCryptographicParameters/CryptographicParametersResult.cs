namespace EBVL.Shared.Dto.Modules.Examples.Cryptography.GenerateCryptographicParameters;

public sealed record CryptographicParametersResult
{
    public required string Key { get; init; }
    public required string Tweak { get; init; }
}
