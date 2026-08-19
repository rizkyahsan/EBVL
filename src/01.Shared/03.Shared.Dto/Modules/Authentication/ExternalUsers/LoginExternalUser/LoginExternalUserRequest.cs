namespace EBVL.Shared.Dto.Modules.Authentication.ExternalUsers.LoginExternalUser;

public record LoginExternalUserRequest
{
    public required string Username { get; set; }
    public required string Password { get; set; }
}
