namespace EBVL.Shared.Dto.Modules.Authentication.ExternalUsers.VerifiedExternalUser;

public sealed record VerifiedExternalUserResult
{
    public bool Succeeded { get; init; }
    public string? ErrorMessage { get; init; }
    public string? UserToken { get; init; }
}
