using System.Security.Claims;

namespace EBVL.BackEnd.Services.LocalIdentity.Model;

public sealed record LoginResult
{
    public string? ErrorMessage { get; private set; }
    public IEnumerable<Claim> Claims { get; private set; }
    public bool Succeeded { get; private set; }
    public Guid IdentityUserId { get; private set; }

    private LoginResult(bool succeeded, Guid identityUserId)
    {
        Claims = new List<Claim>();
        Succeeded = succeeded;
        IdentityUserId = identityUserId;
    }

    private LoginResult(bool succeeded, IEnumerable<Claim> claims, string? errorMessage)
    {
        Succeeded = succeeded;
        Claims = claims;
        ErrorMessage = errorMessage;
    }

    public static LoginResult Success(Guid identityUserId)
    {
        return new(true, identityUserId);
    }

    public static LoginResult Success(IEnumerable<Claim> claims)
    {
        return new(true, claims, null);
    }

    public static LoginResult Failed(string errorMessage)
    {
        return new(false, [], errorMessage);
    }
}
