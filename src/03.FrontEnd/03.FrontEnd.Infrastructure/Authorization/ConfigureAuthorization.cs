using Pertamina.Extensions.Identity.Statics;

namespace EBVL.FrontEnd.Infrastructure.Authorization;

public static class ConfigureAuthorization
{
    public static IServiceCollection AddAuthorizationService(this IServiceCollection services)
    {
        _ = services.AddAuthorization(options =>
        {
            foreach (var permission in MainPermissions.All)
            {
                options.AddPolicy(permission, policy => policy.RequireClaim(ClaimTypeFor.Permission, permission));
            }

            foreach (var permission in AdministrationPermissions.All)
            {
                options.AddPolicy(permission, policy => policy.RequireClaim(ClaimTypeFor.Permission, permission));
            }

            foreach (var permission in MasterDataPermissions.All)
            {
                options.AddPolicy(permission, policy => policy.RequireClaim(ClaimTypeFor.Permission, permission));
            }
        });

        return services;
    }
}
