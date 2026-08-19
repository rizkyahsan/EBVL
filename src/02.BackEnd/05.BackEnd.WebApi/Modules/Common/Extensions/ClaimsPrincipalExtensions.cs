using System.Security.Claims;

namespace EBVL.BackEnd.WebApi.Modules.Common.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static bool HasPermission(this ClaimsPrincipal user, string permission)
    {
        return user.Claims.Any(c => c.Type == "scope" &&
            c.Value == permission);
    }
}
