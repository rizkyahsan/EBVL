namespace EBVL.Shared.Dto.Modules.Authentication.ExternalUsers.SendOtpExternalUser;

public sealed record SendOtpExternalUserResult
{
    public required string Message { get; init; }
}
