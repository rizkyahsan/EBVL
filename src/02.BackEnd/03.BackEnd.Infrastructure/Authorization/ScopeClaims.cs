using System.Security.Claims;

namespace EBVL.BackEnd.Infrastructure.Authorization;

public static class ScopeClaims
{
    private static readonly string[] _claimTypes = ["scp", "scope", "permission"];

    public static bool ContainsAny(ClaimsPrincipal user, params string[] acceptedScopes)
    {
        ArgumentNullException.ThrowIfNull(user);
        ArgumentNullException.ThrowIfNull(acceptedScopes);

        return GetValues(user).Any(scope => acceptedScopes.Contains(scope, StringComparer.Ordinal));
    }

    public static IEnumerable<string> GetValues(ClaimsPrincipal user)
    {
        ArgumentNullException.ThrowIfNull(user);

        return user.Claims
            .Where(claim => _claimTypes.Contains(claim.Type, StringComparer.OrdinalIgnoreCase))
            .SelectMany(claim => claim.Value.Split([' ', ','], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            .Select(value => value.Trim('[', ']', '"'))
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .Distinct(StringComparer.Ordinal);
    }
}
