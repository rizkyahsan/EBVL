using System.Security.Claims;
using EBVL.BackEnd.Infrastructure.Authorization;
using EBVL.BackEnd.Services.CurrentUser;
using Microsoft.AspNetCore.Http;

namespace EBVL.BackEnd.Infrastructure.CurrentUser;

public sealed class HttpContextRequestActor(IHttpContextAccessor httpContextAccessor) : IRequestActor
{
    private ClaimsPrincipal User => httpContextAccessor.HttpContext?.User
        ?? throw new UnauthorizedAccessException("An authenticated user is required.");

    public string Username => User.FindFirst("preferred_username")?.Value
        ?? User.FindFirst("email")?.Value
        ?? User.FindFirst("name")?.Value
        ?? throw new UnauthorizedAccessException("The authenticated user has no username claim.");

    public IReadOnlySet<string> Roles => User.Claims
        .Where(claim => claim.Type.Equals("roles", StringComparison.OrdinalIgnoreCase)
            || claim.Type.Equals("role", StringComparison.OrdinalIgnoreCase)
            || claim.Type.Equals(ClaimTypes.Role, StringComparison.OrdinalIgnoreCase))
        .SelectMany(claim => claim.Value.Split([','], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        .Select(value => value.Trim('[', ']', '"'))
        .ToHashSet(StringComparer.Ordinal);

    public IReadOnlySet<string> Scopes => ScopeClaims.GetValues(User).ToHashSet(StringComparer.Ordinal);
}
