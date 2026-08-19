namespace EBVL.Shared.Dto.Modules.Authentication.ExternalUsers.LoginExternalUser;

public sealed record LoginExternalUserResult
{
    public bool Succeeded { get; init; }
    public string? ErrorMessage { get; init; }
    public bool RequireOtp { get; init; }
    public Guid? ExternalLoginId { get; init; }
}
