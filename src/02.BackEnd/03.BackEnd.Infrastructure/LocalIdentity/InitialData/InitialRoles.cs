using EBVL.BackEnd.Infrastructure.LocalIdentity.Models;

namespace EBVL.BackEnd.Infrastructure.LocalIdentity.InitialData;

public static class InitialRoles
{
    public static readonly Guid AdminConcurencyStamp =
       Guid.Parse("7F754C8B-1B06-4F30-A269-D93674BBBD39");
    public static readonly AspNetCoreRole[] All =
    [
        new()
        {
            Name = "Lender",
            NormalizedName = "LENDER",
            ConcurrencyStamp = AdminConcurencyStamp.ToString()
        }
    ];
}
