using System.Security.Cryptography;

namespace EBVL.BackEnd.Logics.Modules.Administration.VendorRegistrations;

internal static class PasswordHasher
{
    private const int Iterations = 210_000;
    private const int SaltSize = 32;
    private const int HashSize = 32;

    public static (string Hash, string Salt) Hash(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, HashAlgorithmName.SHA256, HashSize);
        return (Convert.ToBase64String(hash), Convert.ToBase64String(salt));
    }

    public static bool Verify(string password, string expectedHash, string encodedSalt)
    {
        var salt = Convert.FromBase64String(encodedSalt);
        var actualHash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, HashAlgorithmName.SHA256, HashSize);
        return CryptographicOperations.FixedTimeEquals(actualHash, Convert.FromBase64String(expectedHash));
    }
}
