using EBVL.BackEnd.Domain.Security;

namespace EBVL.BackEnd.Logics.Modules.Administration.VendorRegistrations;

internal static class PasswordHasher
{
    public static (string Hash, string Salt) Hash(string password)
    {
        return VendorPasswordHasher.Hash(password);
    }

    public static bool Verify(string password, string expectedHash, string encodedSalt)
    {
        return VendorPasswordHasher.Verify(password, expectedHash, encodedSalt);
    }
}
