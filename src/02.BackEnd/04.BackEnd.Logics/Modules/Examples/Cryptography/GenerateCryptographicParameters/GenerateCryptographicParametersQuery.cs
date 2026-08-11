using System.Security.Cryptography;
using EBVL.Shared.Dto.Modules.Examples.Cryptography.GenerateCryptographicParameters;

namespace EBVL.BackEnd.Logics.Modules.Examples.Cryptography.GenerateCryptographicParameters;

public sealed class GenerateCryptographicParametersQuery : IRequest<GenerateCryptographicParametersResponse>
{
}

public sealed class GenerateCryptographicParametersQueryHandler
    : IRequestHandler<GenerateCryptographicParametersQuery, GenerateCryptographicParametersResponse>
{
    public Task<GenerateCryptographicParametersResponse> Handle(GenerateCryptographicParametersQuery request, CancellationToken cancellationToken)
    {

        var response = new GenerateCryptographicParametersResponse
        {
            Item = new CryptographicParametersResult
            {
                Key = GenerateRandomKey(),
                Tweak = GenerateRandomTweak()
            }
        };

        return Task.FromResult(response);
    }

    private static string GenerateRandomKey()
    {
        using var aes = Aes.Create();
        aes.KeySize = 256;
        aes.GenerateKey();

        return Convert.ToBase64String(aes.Key);
    }

    private static string GenerateRandomTweak(int minLengthBytes = 16, int maxLengthBytes = 64)
    {
        var randomNumberGenerator = RandomNumberGenerator.Create();
        var length = new Random().Next(minLengthBytes, maxLengthBytes + 1);
        var tweakBytes = new byte[length];

        randomNumberGenerator.GetBytes(tweakBytes);

        return Convert.ToBase64String(tweakBytes);
    }
}
