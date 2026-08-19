using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using EBVL.BackEnd.Infrastructure.LocalIdentity.Models;
using EBVL.BackEnd.Infrastructure.LocalIdentity.Statics;

namespace EBVL.BackEnd.Infrastructure.LocalIdentity;

public class AspNetLocalIdentityDatabase(DbContextOptions<AspNetLocalIdentityDatabase> options)
    : IdentityDbContext<AspNetCoreUser, AspNetCoreRole, Guid>(options)
{
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        _ = builder.HasDefaultSchema(SchemaNameFor.LocalIdentity);
        _ = builder.Entity<AspNetCoreRole>(x => x.ToTable(TableNameFor.Roles));
        _ = builder.Entity<IdentityRoleClaim<Guid>>(x => x.ToTable(TableNameFor.RoleClaims));
        _ = builder.Entity<AspNetCoreUser>(x => x.ToTable(TableNameFor.Users));
        _ = builder.Entity<IdentityUserClaim<Guid>>(x => x.ToTable(TableNameFor.UserClaims));
        _ = builder.Entity<IdentityUserLogin<Guid>>(x => x.ToTable(TableNameFor.UserLogins));
        _ = builder.Entity<IdentityUserToken<Guid>>(x => x.ToTable(TableNameFor.UserTokens));
        _ = builder.Entity<IdentityUserRole<Guid>>(x => x.ToTable(TableNameFor.UserRoles));
    }
}
