using Microsoft.AspNetCore.Identity;

namespace EBVL.BackEnd.Infrastructure.LocalIdentity.InitialData;

public static class InitialRoleClaim
{
    public static readonly IdentityRoleClaim<Guid>[] All =
    [
        new()
        {
            Id = 1,
            ClaimType = "Permission",
            ClaimValue = "ebvl.api.audience"
        }
        ,
        new()
        {
            Id = 2,
            ClaimType = "Permission",
            ClaimValue = "fino.mp"
        }
        ,
        new()
        {
            Id = 3,
            ClaimType = "Permission",
            ClaimValue = "fino.mp.mp"
        }
        ,
        new()
        {
            Id = 4,
            ClaimType = "Permission",
            ClaimValue = "fino.mp.mp.read"
        }
        ,
        new()
        {
            Id = 5,
            ClaimType = "Permission",
            ClaimValue = "fino.mp.mp.write"
        }
        ,
        new()
        {
            Id = 6,
            ClaimType = "Permission",
            ClaimValue = "fino.mp.mp.download"
        }
        ,
        new()
        {
            Id = 7,
            ClaimType = "Permission",
            ClaimValue = "fino.mp.mp.upload"
        }
    ];
}
