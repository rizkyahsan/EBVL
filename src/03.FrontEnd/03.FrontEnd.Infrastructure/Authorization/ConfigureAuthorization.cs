using Pertamina.Extensions.Identity.Statics;
using EBVL.Shared.Dto.Modules.MasterData.Documents;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace EBVL.FrontEnd.Infrastructure.Authorization;

public static class ConfigureAuthorization
{
    public static IServiceCollection AddAuthorizationService(this IServiceCollection services)
    {
        _ = services.AddAuthorization(options =>
        {
            foreach (var permission in MainPermissions.All)
            {
                AddPermissionPolicy(options, permission);
            }

            foreach (var permission in AdministrationPermissions.All)
            {
                AddPermissionPolicy(options, permission);
            }

            foreach (var permission in QuestionnairePermissions.All)
            {
                AddPermissionPolicy(options, permission, permission == QuestionnairePermissions.View ? QuestionnairePermissions.Manage : null);
            }

            foreach (var permission in DocumentPermissions.All)
            {
                AddPermissionPolicy(options, permission, permission == DocumentPermissions.View ? DocumentPermissions.Manage : null);
            }
        });

        return services;
    }

    private static void AddPermissionPolicy(AuthorizationOptions options, string permission, string? impliedPermission = null)
    {
        options.AddPolicy(permission, policy => policy.RequireAssertion(context =>
            context.User.Claims.Any(claim => claim.Type == ClaimTypeFor.Permission && claim.Value == permission)
            || (impliedPermission is not null && context.User.Claims.Any(claim => claim.Type == ClaimTypeFor.Permission && claim.Value == impliedPermission))
            || context.User.Claims.Any(claim => claim.Type is ClaimTypes.Role or "role"
                && claim.Value.Equals("Administrator", StringComparison.OrdinalIgnoreCase))));
    }
}
