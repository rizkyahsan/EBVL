namespace EBVL.Shared.Dto.Modules.Authentication.ExternalUsers.CheckExternalUser;

public sealed record UserItem
{
    public required bool IsVerified { get; init; }
}
